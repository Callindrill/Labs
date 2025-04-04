using Nico.Labs.IntroMutationTesting;

namespace IntroMutationTesting.Tests.MutantKillers;

public class TryApplyDiscountCodeTests
{
    [Fact]
    public void TryApplyDiscountCode_NonSaveCodeShouldNotRemoveExistingSaveDiscount()
    {
        // Kills the "&&" => "||" mutant in line 45
        var invoice = new Invoice(100, 0.08m);

        Assert.True(invoice.TryApplyDiscountCode("SAVE10"));
        Assert.Contains("SAVE10", invoice.DiscountCodes);

        // Now apply "SHIPFREE" (does NOT start with "SAVE")
        Assert.True(invoice.TryApplyDiscountCode("SHIPFREE"));
        // If the code was mutated to ||, that would remove the "SAVE10" discount
        Assert.Contains("SAVE10", invoice.DiscountCodes);
    }

    [Fact]
    public void TryApplyDiscountCode_SaveCodeShouldNotRemoveNonSaveCode()
    {
        var invoice = new Invoice(100, 0.08m);

        Assert.True(invoice.TryApplyDiscountCode("SHIPFREE"));
        Assert.Contains("SHIPFREE", invoice.DiscountCodes);

        // Now apply "SHIPFREE" (does NOT start with "SAVE")
        Assert.True(invoice.TryApplyDiscountCode("SAVE10"));

        // 
        Assert.Contains("SHIPFREE", invoice.DiscountCodes);
        Assert.Contains("SAVE10", invoice.DiscountCodes);
    }

    [Fact]
    public void TryApplyDiscountCode_SaveCodeShouldRemoveSaveCodeAndNotRemoveNonSaveCode()
    {
        var invoice = new Invoice(100, 0.08m);

        Assert.True(invoice.TryApplyDiscountCode("SHIPFREE"));
        Assert.Contains("SHIPFREE", invoice.DiscountCodes);

        // Now apply "SHIPFREE" (does NOT start with "SAVE")
        Assert.True(invoice.TryApplyDiscountCode("SAVE10"));
        Assert.Contains("SHIPFREE", invoice.DiscountCodes);
        Assert.Contains("SAVE10", invoice.DiscountCodes);

        // Now apply "SHIPFREE" (does NOT start with "SAVE")
        Assert.True(invoice.TryApplyDiscountCode("SAVE20"));
        Assert.Contains("SHIPFREE", invoice.DiscountCodes);
        Assert.Contains("SAVE20", invoice.DiscountCodes);
        Assert.DoesNotContain("SAVE10", invoice.DiscountCodes);
    }
}
