using Infrastructure.Models;

namespace Infrastructure.Helpers;

public class Validators
{
    public static ValidatorResponse<string> Name(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return ValidatorResponse<string>.Failed("Please enter a Name.");

        return ValidatorResponse<string>.Success(name);
    }
    public static ValidatorResponse<decimal> StrToDecimal(string? decimalStr)
    {
        bool isDecimal = decimal.TryParse(decimalStr, out decimal dec);

        if (!isDecimal) return ValidatorResponse<decimal>.Failed("Please enter a valid number");

        return ValidatorResponse<decimal>.Success(dec);
    }
}
