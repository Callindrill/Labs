using Nico.Labs.IntroMutationTesting;

namespace IntroMutationTesting.Tests.MutantKillers;

public class CalculateTotalTests
{
    [Fact]
    public void CalculateTotal_HighSubtotalWithFreeShipping_NoFreeShipping()
    {
        // Arrange
        // let's make sure to be AT the boundary condition for the conditional statement (50 instead of some arbitrarily high number).
        var invoice = new Invoice(50, 0.05m);

        // Act
        invoice.TryApplyDiscountCode("SHIPFREE");
        var total = invoice.CalculateTotal();

        // 50 subtotal - 0 discount + 2.50 tax (5% of 50) + 5 shipping => 57.50
        Assert.Equal(57.500m, total);
    }
}