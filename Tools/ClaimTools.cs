namespace BankWebApp.Tools;

public static class ClaimTools
{
    // for when you are certain that the string is an int (for example in a claim)
    public static int ToInt32(this string str) => int.Parse(str);
}