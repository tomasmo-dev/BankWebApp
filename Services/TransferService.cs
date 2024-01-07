namespace BankWebApp.Services;
/// <summary>
/// This class provides services for transferring money between bank accounts.
/// </summary>
public class TransferService
{
    private readonly DatabaseService _databaseService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransferService"/> class.
    /// </summary>
    public TransferService()
    {
        _databaseService = new DatabaseService();
    }

    /// <summary>
    /// Transfers money from one account to another.
    /// </summary>
    /// <param name="FromAcc">The account number to transfer money from.</param>
    /// <param name="ToAcc">The account number to transfer money to.</param>
    /// <param name="Amount">The amount of money to transfer.</param>
    /// <returns>A tuple containing a boolean indicating success or failure, and a string containing the reason for failure.</returns>
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

    /// <summary>
    /// Adds funds to a specified account.
    /// </summary>
    /// <param name="AccountNumber">The account number to add funds to.</param>
    /// <param name="Amount">The amount of money to add.</param>
    public void PrintMoney(string AccountNumber, decimal Amount)
    {
        var account = _databaseService.GetBankAccountById(AccountNumber)!;

        Guid guid = Guid.Parse(account.AccountNumber);

        _databaseService.AddFunds(guid, Amount);
    }
}