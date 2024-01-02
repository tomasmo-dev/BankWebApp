namespace BankWebApp.Models;

public class AccountHistoryModel
{
    public IList<TransactionModel> Transactions { get; set; }
}