// ReSharper disable StringLiteralTypo

using System.Text;

namespace Nico.Labs.IntroMutationTesting;

/// <summary>
/// A simplified invoice calculator that applies discounts, tax, 
/// and possibly shipping costs. Used to illustrate real-world 
/// mutation testing scenarios in .NET.
/// </summary>
public class Invoice
{
    private static readonly string[] AllowedCodes = ["SAVE10", "SAVE20", "SAVE30", "SHIPFREE"];
    private readonly decimal _subtotal;
    private readonly decimal _taxRate;

    private readonly List<string> _discountCodes = [];
    public IReadOnlyList<string> DiscountCodes => _discountCodes.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="Invoice"/> class.
    /// </summary>
    /// <param name="subtotal">Pretax subtotal of the invoice.</param>
    /// <param name="taxRate">Tax as a decimal (e.g., 0.06 for 6% tax).</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if subtotal or taxRate is negative.
    /// </exception>
    public Invoice(decimal subtotal, decimal taxRate)
    {
        if (subtotal < 0)
            throw new ArgumentOutOfRangeException(nameof(subtotal), "Subtotal cannot be negative.");
        if (taxRate < 0)
            throw new ArgumentOutOfRangeException(nameof(taxRate), "Tax rate cannot be negative.");

        _subtotal = subtotal;
        _taxRate = taxRate;
    }

    public bool TryApplyDiscountCode(string? discountCode)
    {
        if (discountCode is null || !AllowedCodes.Contains(discountCode) || _discountCodes.Contains(discountCode))
            return false;

        if (discountCode.StartsWith("SAVE"))
            _discountCodes.RemoveAll(x => x.StartsWith("SAVE"));

        _discountCodes.Add(discountCode);
        return true;
    }

    /// <summary>
    /// Calculates the final total after discount (if any), plus tax, plus shipping (if applicable).
    /// </summary>
    /// <returns>Total invoice amount, rounded to two decimals.</returns>
    public decimal CalculateTotal()
    {
        var discount = GetDiscount();
        var taxedAmount = GetTaxes();
        var shipping = GetShipping();
        var total = _subtotal - discount + taxedAmount + shipping;

        return decimal.Round(total, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Calculates the discount based on an applied provided discount code.
    /// </summary>
    private decimal GetDiscount() =>
        DiscountCodes switch
        {
            _ when DiscountCodes.Contains("SAVE10") => _subtotal * 0.10m,
            _ when DiscountCodes.Contains("SAVE20") => _subtotal * 0.20m,
            _ when DiscountCodes.Contains("SAVE30") => _subtotal * 0.30m,
            _ => 0m
        };

    /// <summary>
    /// Calculates the discount based on an applied provided discount code.
    /// </summary>
    private decimal GetShipping() =>
        DiscountCodes switch
        {
            _ when _subtotal < 50 && DiscountCodes.Contains("SHIPFREE") => 0m,
            _ => 5.00m
        };

    /// <summary>
    /// Calculates the taxes based on the subtotal and tax rate. Discounts are still taxed.
    /// </summary>
    private decimal GetTaxes() => _subtotal * _taxRate;

    public override string ToString()
    {
        var subString = $"{_subtotal:C2}";
        var taxRateString = $"{_taxRate:P2}";
        var taxString = "+" + $"{GetTaxes():C2}".PadLeft(subString.Length + 4);
        var discountString = "-" + $"{GetDiscount():C2}".PadLeft(subString.Length + 4);
        var shipString = GetShipping() == 0m ? "Free Shipping!" : "+" + $"{GetShipping():C2}".PadLeft(subString.Length + 4);
        var totalString = "=" + $"{CalculateTotal():C2}".PadLeft(subString.Length + 4); ;
        var sb = new StringBuilder();

        sb.AppendLine($"|{new string('=', 78)}|");
        sb.AppendLine($"|{new string(' ', 36)}INVOICE{new string(' ', 35)}|");
        sb.AppendLine($"|{new string('=', 78)}|");
        sb.AppendLine($"| SUBTOTAL:{subString.PadLeft(68)}|");
        sb.AppendLine($"| TAXES ({taxRateString}):{taxString.PadLeft(68 - taxRateString.Length)}|");
        if (GetDiscount() != 0m)
            sb.AppendLine($"| DISCOUNT:{discountString.PadLeft(68)}|");
        sb.AppendLine($"| SHIPPING:{shipString.PadLeft(68)}|");
        sb.AppendLine($"|{new string('-', 78)}|");
        sb.AppendLine($"| TOTAL:{totalString.PadLeft(71)}|");
        sb.AppendLine($"|{new string('=', 78)}|");

        return sb.ToString();
    }
}
