using BankWebApp.Models;
using Microsoft.Data.SqlClient;

namespace BankWebApp.Services;

public partial class DatabaseService
{
    public IList<UserModel> GetUsers()
    {
        var users = new List<UserModel>();
        
        var sql = "SELECT [Users].Id, [Users].Username, [Users].PasswordHash, [Users].CreatedAt, [Users].ContactId, [Users].AddressId, " +
                  "[Contacts].Email, [Contacts].PhoneNumber, " +
                  "[Addresses].Street, [Addresses].City, [Addresses].PostCode, [Addresses].Country " +
                  "FROM [Users] " +
                  "INNER JOIN [Contacts] ON [Users].[ContactId] = [Contacts].[Id] " +
                  "INNER JOIN [Addresses] ON [Users].[AddressId] = [Addresses].[Id]";
        var cmd = new SqlCommand(sql, _connection);
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            users.Add(new UserModel
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                CreatedAt = reader.GetDateTime(3),
                Contact = new ContactModel
                {
                    Email = reader.GetString(6),
                    PhoneNumber = reader.GetString(7)
                },
                Address = new AddressModel
                {
                    Street = reader.GetString(8),
                    City = reader.GetString(9),
                    PostCode = reader.GetString(10),
                    Country = reader.GetString(11)
                }
            });
        }

        return users;
    }

    public bool UsernameExists(string _username)
    {
        var sql = "SELECT Username FROM Users WHERE Username = @username";
        var cmd = new SqlCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@username", _username);
        var result = cmd.ExecuteScalar();

        return result != null;
    }

    public bool RegisterUser(UserModel user)
    {
        var transaction = _connection.BeginTransaction();

        try
        {
            // Insert contact
            var sql = "INSERT INTO Contacts (Email, PhoneNumber) OUTPUT INSERTED.Id VALUES (@Email, @PhoneNumber)";
            var cmd = new SqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@Email", user.Contact.Email);
            cmd.Parameters.AddWithValue("@PhoneNumber", user.Contact.PhoneNumber);
            cmd.Transaction = transaction;
            var contactId = (int)cmd.ExecuteScalar();

            // Insert address
            sql = "INSERT INTO Addresses (Street, City, PostCode, Country) OUTPUT INSERTED.Id VALUES (@Street, @City, @PostCode, @Country)";
            cmd = new SqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@Street", user.Address.Street);
            cmd.Parameters.AddWithValue("@City", user.Address.City);
            cmd.Parameters.AddWithValue("@PostCode", user.Address.PostCode);
            cmd.Parameters.AddWithValue("@Country", user.Address.Country);
            cmd.Transaction = transaction;
            var addressId = (int)cmd.ExecuteScalar();

            // Insert user
            sql = "INSERT INTO Users (Username, PasswordHash, CreatedAt, ContactId, AddressId) OUTPUT INSERTED.Id VALUES (@Username, @PasswordHash, @CreatedAt, @ContactId, @AddressId)";
            cmd = new SqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@ContactId", contactId);
            cmd.Parameters.AddWithValue("@AddressId", addressId);
            cmd.Transaction = transaction;
            var userId = cmd.ExecuteScalar();
            
            
            sql = "INSERT INTO BankAccount (UserId, AccountNumber, Balance) VALUES (@UserId, @AccountNumber, 0)";
            cmd = new SqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@AccountNumber", Guid.NewGuid());
            cmd.Transaction = transaction;
            cmd.ExecuteNonQuery();
            
            transaction.Commit();

            return true;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            return false;
        }
    }
}