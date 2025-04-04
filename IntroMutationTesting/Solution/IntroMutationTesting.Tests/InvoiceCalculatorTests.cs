namespace Nico.Labs.IntroMutationTesting.Tests;

public class InvoiceTests
{
    [Theory]
    [InlineData(100, 0.08, null, 113.00, false)]   // No discount, tax only
    [InlineData(100, 0.08, "SAVE10", 103.00, true)] // 10% discount, then 8% tax
    public void CalculateTotal_BasicScenarios(decimal subtotal, decimal tax, string code, decimal expected, bool discountApplied)
    {
        var invoice = new Invoice(subtotal, tax);
        var applied = invoice.TryApplyDiscountCode(code);
        if (discountApplied)
            Assert.Contains(code, invoice.DiscountCodes);
        else
            Assert.DoesNotContain(code, invoice.DiscountCodes);
        Assert.Equal(discountApplied, applied);
        Assert.Equal(expected, invoice.CalculateTotal());
    }

    [Fact]
    public void CalculateTotal_LowSubtotalWithNoCode_ShouldAddShipping()
    {
        var invoice = new Invoice(40, 0.05m);
        var total = invoice.CalculateTotal();
        // 40 + 5% tax = 42, plus shipping 5 => 47
        Assert.Equal(47.00m, total);
    }

    [Fact]
    public void Ctor_NegativeSubtotal_ThrowsArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Invoice(-1, 0.08m));
    }

    [Theory]
    [InlineData("SAVE10")]
    [InlineData("SAVE20")]
    [InlineData("SAVE30")]
    [InlineData("SHIPFREE")]
    public void TryApplyDiscountCode_KnownCodesRecognized(string code)
    {
        var invoice = new Invoice(50, 0.10m);
        Assert.True(invoice.TryApplyDiscountCode(code));
    }

    [Fact]
    public void TryApplyDiscountCode_UnknownCodeNotRecognized()
    {
        var invoice = new Invoice(50, 0.10m);
        Assert.False(invoice.TryApplyDiscountCode(Guid.Empty.ToString()));
    }

    [Theory]
    [InlineData("SAVE10", "SAVE20")]
    [InlineData("SAVE10", "SAVE30")]
    [InlineData("SAVE20", "SAVE10")]
    [InlineData("SAVE20", "SAVE30")]
    [InlineData("SAVE30", "SAVE10")]
    [InlineData("SAVE30", "SAVE20")]
    public void TryApplyDiscountCode_DiscountsReplace(string code1, string code2)
    {
        var invoice = new Invoice(50, 0.10m);
        Assert.True(invoice.TryApplyDiscountCode(code1));
        Assert.True(invoice.TryApplyDiscountCode(code2));
        Assert.NotNull(invoice.DiscountCodes);
        Assert.Single(invoice.DiscountCodes);
        Assert.DoesNotContain(code1, invoice.DiscountCodes);
        Assert.Contains(code2, invoice.DiscountCodes);
    }

    [Fact]
    public void Ctor_NegativeTaxRate_ThrowsArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Invoice(100, -0.01m));
    }

    [Theory]
    [InlineData("SAVE20", 95.00)]
    [InlineData("SAVE30", 85.00)]
    public void CalculateTotal_WithBiggerDiscounts_ShouldApplyCorrectly(string code, decimal expected)
    {
        // Arrange
        var invoice = new Invoice(100, 0.10m);

        // Act
        invoice.TryApplyDiscountCode(code);
        var total = invoice.CalculateTotal();

        // Assert
        Assert.Equal(expected, total);
    }

    [Fact]
    public void CalculateTotal_LowSubtotalWithFreeShipping_NoShipping()
    {
        // Arrange
        var invoice = new Invoice(40, 0.05m);

        // Act
        invoice.TryApplyDiscountCode("SHIPFREE");
        var total = invoice.CalculateTotal();

        // 40 subtotal - 0 discount + 2 tax (5% of 40) + 0 shipping => 42
        Assert.Equal(42.00m, total);
    }

    [Fact]
    public void ToString_WithDiscountRendersCorrectInvoice()
    {
        // Arrange
        const decimal taxRate = 0.08m;
        const decimal subTotal = 496;
        var invoice = new Invoice(subTotal, taxRate);
        // Apply a discount just to vary output
        invoice.TryApplyDiscountCode("SAVE20");
        var total = invoice.CalculateTotal();

        // Act
        var result = invoice.ToString();
        var expected =
              "|==============================================================================|\r\n"
            + "|                                    INVOICE                                   |\r\n"
            + "|==============================================================================|\r\n"
            + "| SUBTOTAL:                                                             $496.00|\r\n"
            + "| TAXES (8.00%):                                                   +     $39.68|\r\n"
            + "| DISCOUNT:                                                        -     $99.20|\r\n"
            + "| SHIPPING:                                                        +      $5.00|\r\n"
            + "|------------------------------------------------------------------------------|\r\n"
            + "| TOTAL:                                                           =    $441.48|\r\n"
            + "|==============================================================================|\r\n";

        Assert.Equal(expected, result);
    }
}