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
}