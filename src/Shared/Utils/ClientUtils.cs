using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Shared.Utils;

public static class ClientUtils
{
    public static string GenerateToken(int length)
    {
        using var random = RandomNumberGenerator.Create();
        byte[] tokenBuffer = new byte[length];
        random.GetBytes(tokenBuffer);
        var hash = Convert.ToBase64String(tokenBuffer);
        return Regex.Replace(hash, "[^0-9a-zA-Z]+", "");
    }
}