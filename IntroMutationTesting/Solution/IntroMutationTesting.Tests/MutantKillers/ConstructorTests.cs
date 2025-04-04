using Nico.Labs.IntroMutationTesting;

namespace IntroMutationTesting.Tests.MutantKillers;

public class ConstructorTests
{
    [Fact]
    public void Ctor_ZeroSubtotal_DoesNotThrow()
    {
        // Kills the line 31 (subtotal < 0 => <= 0) mutant
        var invoice = new Invoice(0, 0.08m);
        invoice.TryApplyDiscountCode("SHIPFREE");
        Assert.NotNull(invoice);
        Assert.Equal(0.00m, invoice.CalculateTotal());
    }

    [Fact]
    public void Ctor_ZeroTaxRate_DoesNotThrow()
    {
        // Kills the line 33 (taxRate < 0 => <= 0) mutant
        var invoice = new Invoice(100, 0m);
        Assert.NotNull(invoice);
        // 100 + 0 tax + 5 shipping => 105.00
        Assert.Equal(105.00m, invoice.CalculateTotal());
    }

    [Fact]
    public void Ctor_NegativeSubtotal_ThrowsWithCorrectMessage()
    {
        // Kills the string mutation "Subtotal cannot be negative." => ""
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Invoice(-10, 0.08m));
        Assert.Contains("Subtotal cannot be negative.", ex.Message);
    }

    [Fact]
    public void Ctor_NegativeTaxRate_ThrowsWithCorrectMessage()
    {
        // Kills the string mutation "Tax rate cannot be negative." => ""
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Invoice(100, -0.01m));
        Assert.Contains("Tax rate cannot be negative.", ex.Message);
    }



    //[Fact]
    //public void CalculateTotal_SubtotalAt50_ShouldStillChargeShippingWithoutShipFree()
    //{
    //    // Kills the line 84 mutation (_subtotal < 50 => <= 50)
    //    var invoice = new Invoice(50, 0.05m);
    //    var total = invoice.CalculateTotal();

    //    // 50 + 2.50 tax + 5 shipping => 57.50
    //    Assert.Equal(57.50m, total);
    //}

    //[Fact]
    //public void ToString_ShouldShowPlusSignOnShippingWhenNotFree()
    //{
    //    // Possibly needed to kill the line 97 or 99 string mutation
    //    // if Stryker changes "+" to "" in shipping for non-free scenario
    //    var invoice = new Invoice(60, 0.05m);
    //    // So shipping is not free (60 >= 50 means shipping=5)
    //    var result = invoice.ToString();

    //    var lines = result.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    //    var shippingLine = lines.FirstOrDefault(l => l.Contains("SHIPPING"));

    //    Assert.Contains("+", shippingLine);
    //    Assert.Contains("$5.00", shippingLine);
    //}

    //[Fact]
    //public void ToString_WhenShippingIsFree_DisplaysFreeShippingMessage()
    //{
    //    // Kills the "Free Shipping!" => "" mutation on line 99
    //    var invoice = new Invoice(30, 0.05m);  // Subtotal < 50
    //    invoice.TryApplyDiscountCode("SHIPFREE");

    //    var rendered = invoice.ToString();
    //    Assert.Contains("Free Shipping!", rendered);
    //}

}

