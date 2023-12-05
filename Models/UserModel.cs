namespace BankWebApp.Models;

/// <summary>
/// Represents a User in the system.
/// </summary>
public class UserModel
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Username of the user.
    /// </summary>
    public string Username { get; set; }
    /// <summary>
    /// Hashed password of the user. with bcrypt
    /// </summary>
    public string PasswordHash { get; set; }
    /// <summary>
    /// A date when the user was created. (in database)
    /// </summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// Contact model associated with the user.
    /// </summary>
    public ContactModel Contact { get; set; }
    /// <summary>
    /// Address model associated with the user.
    /// </summary>
    public AddressModel Address { get; set; }
}