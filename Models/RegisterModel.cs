using System.ComponentModel.DataAnnotations;

namespace BankWebApp.Models;

public class RegisterModel
{
    [MaxLength(50)]
    public string Username { get; set; }
    
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    
    [EmailAddress]
    public string Email { get; set; }
    [Phone]
    public string PhoneNumber { get; set; }
    
    [MaxLength(50)]
    public string Street { get; set; }
    [MaxLength(45)]
    public string City { get; set; }
    [MaxLength(45)]
    public string PostCode { get; set; }
    [MaxLength(45)]
    public string Country { get; set; }
    
    
    public bool? Success { get; set; }
    public string? Reason { get; set; }
}