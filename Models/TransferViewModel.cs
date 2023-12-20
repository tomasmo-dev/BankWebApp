namespace BankWebApp.Models;

public class TransferViewModel
{
    public IList<BankAccountModel> BankAccounts { get; set; }
    
    public string FromAccountId { get; set; }
    public string ToAccountId { get; set; }
    
    public decimal Amount { get; set; }
    
    public bool? Success { get; set; }
    public string? Reason { get; set; }

    public static TransferViewModel Empty => new TransferViewModel();
}