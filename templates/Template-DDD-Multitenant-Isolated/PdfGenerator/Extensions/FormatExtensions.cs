using System.Text.RegularExpressions;

namespace PdfGenerator.Extensions;

public static class FormatExtensions
{
    public static string FormatPhone(this string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return "N/A";
        
        var digits = Regex.Replace(phoneNumber, @"[^\d]", "");
        
        if (digits.Length != 10)
            return phoneNumber;

        return $"({digits.Substring(0, 2)}) {digits.Substring(2, 4)} - {digits.Substring(6, 4)}";
    }
}