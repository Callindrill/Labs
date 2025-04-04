using Nico.Labs.IntroMutationTesting;

namespace IntroMutationTesting.Tests.MutantKillers;

public class ToStringTests
{
    [Fact]
    public void ToString_FreeShippingNoDiscountRendersCorrectInvoice()
    {
        // Arrange
        const decimal taxRate = 0.08m;
        const decimal subTotal = 49;
        var invoice = new Invoice(subTotal, taxRate);
        // Apply a discount just to vary output
        invoice.TryApplyDiscountCode("SHIPFREE");
        var total = invoice.CalculateTotal();

        // Act
        var result = invoice.ToString();
        var expected =
              "|==============================================================================|\r\n"
            + "|                                    INVOICE                                   |\r\n"
            + "|==============================================================================|\r\n"
            + "| SUBTOTAL:                                                              $49.00|\r\n"
            + "| TAXES (8.00%):                                                    +     $3.92|\r\n"
            + "| SHIPPING:                                                      Free Shipping!|\r\n"
            + "|------------------------------------------------------------------------------|\r\n"
            + "| TOTAL:                                                            =    $52.92|\r\n"
            + "|==============================================================================|\r\n";

        Assert.Equal(expected, result);
    }
}