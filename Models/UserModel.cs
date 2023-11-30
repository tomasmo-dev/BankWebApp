namespace BankWebApp.Models;

public class UserModel
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public ContactModel Contact { get; set; }
    public AddressModel Address { get; set; }
}