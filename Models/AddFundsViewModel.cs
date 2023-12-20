namespace BankWebApp.Models;

public class AddFundsViewModel
{
    public IList<BankAccountModel> BankAccounts { get; set; }
    
    public decimal Amount { get; set; }
    public string SelectedBankAccountNumber { get; set; }
    
    public bool? Success { get; set; }
    public string? Reason { get; set; }
}