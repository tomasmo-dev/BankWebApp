namespace BankWebApp.Models;

public class TransactionModel
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public BankAccountModel Sender { get; set; }
    public int ReceiverId { get; set; }
    public BankAccountModel Receiver { get; set; }
    public decimal Amount { get; set; }
    public DateTime SentAt { get; set; }
}