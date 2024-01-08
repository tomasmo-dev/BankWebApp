namespace BankWebApp.Models;

public class ListUsersViewModel
{
    public UserModel UserModel { get; set; }
    public IList<BankAccountModel> BankAccounts { get; set; }
    
    public IList<TransactionModel> Transactions { get; set; }
}