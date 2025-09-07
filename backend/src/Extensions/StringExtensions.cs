using System;
using System.Text;

namespace Database.Extensions;

public static class StringExtensions
{
    public static string Base64Encode(this string plainText)
    {
        return Convert.ToBase64String(
            Encoding.UTF8.GetBytes(plainText)
        );
    }

    public static string Base64Decode(this string base64EncodedData)
    {
        return Encoding.UTF8.GetString(
            Convert.FromBase64String(base64EncodedData)
        );
    }
}