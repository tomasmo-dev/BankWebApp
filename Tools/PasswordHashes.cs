using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

namespace BankWebApp.Tools;

public static class PasswordHashes
{
    public static string HashPassword(this string text)
    {
        return BCrypt.Net.BCrypt.HashPassword(text);
    }

    public static bool VerifyPassword(string unhashedp, string p2)
    {
        return BCrypt.Net.BCrypt.Verify(unhashedp, p2);
    }
}