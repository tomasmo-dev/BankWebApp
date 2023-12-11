namespace BankWebApp.Models;

public class AccountIndexModel
{
    public UserModel SignedInUser { get; set; }
    public IList<BankAccountModel> BankAccounts { get; set; }
}