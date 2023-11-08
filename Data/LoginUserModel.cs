namespace BankApp.Data
{
    public class LoginUserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        // password hashed with sha256
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ContactId { get; set; }
        public int AddressId { get; set; }
    }
}
