namespace BankWebApp.Services;

public class TransferService
{
    private readonly DatabaseService _databaseService;

    public TransferService()
    {
        _databaseService = new DatabaseService();
    }

    public (bool Success, string Reason) TransferMoney(string FromAcc, string ToAcc, decimal Amount)
    {
        var fromAcc = _databaseService.GetBankAccountById(FromAcc);
        var toAcc = _databaseService.GetBankAccountById(ToAcc);

        if (fromAcc == null)
        {
            return (false, "Invalid account to send from.");
        }
        else if (toAcc == null)
        {
            return (false, "Invalid account to send to.");
        }
        
        if (fromAcc.Balance <= Amount)
        {
            return (false, "Insufficient funds.");
        }
        
        Guid fromGuid = Guid.Parse(fromAcc.AccountNumber);
        Guid toGuid = Guid.Parse(toAcc.AccountNumber);

        var success = _databaseService.TransferFunds(fromGuid, toGuid, Amount);
        
        if (!success)
        {
            return (false, "An error occurred while transferring funds.");
        }
        else
        {
            return (true, "");
        }
    }
    
    public void PrintMoney(string AccountNumber, decimal Amount)
    {
        var account = _databaseService.GetBankAccountById(AccountNumber)!;
        
        Guid guid = Guid.Parse(account.AccountNumber);

        _databaseService.AddFunds(guid, Amount);

    }
}