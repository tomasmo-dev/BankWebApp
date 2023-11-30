namespace BankWebApp.Models;

public class BankAccountModel
{
    public int Id { get; set; }
    public string AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public int UserId { get; set; }
    public UserModel User { get; set; }
}