using System.Data;
using BankWebApp.env;
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
        using (SqlConnection con = new SqlConnection(Envs.ConnectionString))
        {
            con.Open();
            var transaction = con.BeginTransaction();

            try
            {

                // Insert contact
                var sql = "INSERT INTO Contacts (Email, PhoneNumber) OUTPUT INSERTED.Id VALUES (@Email, @PhoneNumber)";
                var cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Email", user.Contact.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", user.Contact.PhoneNumber);
                cmd.Transaction = transaction;
                var contactId = (int)cmd.ExecuteScalar();

                // Insert address
                sql =
                    "INSERT INTO Addresses (Street, City, PostCode, Country) OUTPUT INSERTED.Id VALUES (@Street, @City, @PostCode, @Country)";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Street", user.Address.Street);
                cmd.Parameters.AddWithValue("@City", user.Address.City);
                cmd.Parameters.AddWithValue("@PostCode", user.Address.PostCode);
                cmd.Parameters.AddWithValue("@Country", user.Address.Country);
                cmd.Transaction = transaction;
                var addressId = (int)cmd.ExecuteScalar();

                // Insert user
                sql =
                    "INSERT INTO Users (Username, PasswordHash, CreatedAt, ContactId, AddressId) OUTPUT INSERTED.Id VALUES (@Username, @PasswordHash, @CreatedAt, @ContactId, @AddressId)";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                cmd.Parameters.AddWithValue("@ContactId", contactId);
                cmd.Parameters.AddWithValue("@AddressId", addressId);
                cmd.Transaction = transaction;
                var userId = cmd.ExecuteScalar();


                sql = "INSERT INTO BankAccount (UserId, AccountNumber, Balance) VALUES (@UserId, @AccountNumber, 0)";
                cmd = new SqlCommand(sql, con);
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

    public IList<BankAccountModel>? GetBankAccountById(int UserId)
    {
        List<BankAccountModel> accounts = new List<BankAccountModel>();
        
        var sql = "SELECT Id, AccountNumber, Balance, UserId FROM BankAccount WHERE UserId = @Id";
        var cmd = new SqlCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@Id", UserId);
        var reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            accounts.Add(new BankAccountModel
            {
                Id = reader.GetInt32(0),
                AccountNumber = reader.GetString(1),
                Balance = reader.GetDecimal(2),
                UserId = reader.GetInt32(3)
            });
        }

        var users = GetUsers();
        
        foreach (var account in accounts)
        {
            account.User = users.First(u => u.Id == account.UserId);
        }
        
        reader.Close();
        
        return accounts.Count > 0 ? accounts : null;
    }
    
    public BankAccountModel? GetBankAccountById(string Id)
    {
        var sql = "SELECT Id, AccountNumber, Balance, UserId FROM BankAccount WHERE AccountNumber = @Id";
        var cmd = new SqlCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@Id", Id);
        var reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            var result = new BankAccountModel
            {
                Id = reader.GetInt32(0),
                AccountNumber = reader.GetString(1),
                Balance = reader.GetDecimal(2),
                UserId = reader.GetInt32(3)
            };
            
            var users = GetUsers();
            
            result.User = users.First(u => u.Id == result.UserId);
            
            reader.Close();
            
            return result;
        }
        
        return null;
    }

    public BankAccountModel? GetBankAccountByAccountId(int Id)
    {
        var sql = "SELECT Id, AccountNumber, Balance, UserId FROM BankAccount WHERE Id = @Id";
        var cmd = new SqlCommand(sql, _connection);
        
        cmd.Parameters.AddWithValue("@Id", Id);
        
        var reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            var result = new BankAccountModel
            {
                Id = reader.GetInt32(0),
                AccountNumber = reader.GetString(1),
                Balance = reader.GetDecimal(2),
                UserId = reader.GetInt32(3)
            };
            
            var users = GetUsers();
            
            result.User = users.First(u => u.Id == result.UserId);
            
            reader.Close();
            
            return result;
        }
        
        return null;
    }

    public bool TransferFunds(Guid from, Guid To, decimal Amount)
    {
        var fromAccount = GetBankAccountById(from.ToString());
        var toAccount = GetBankAccountById(To.ToString());
        
        using (SqlConnection con = new SqlConnection(Envs.ConnectionString))
        {
            con.Open();
            SqlTransaction transaction = con.BeginTransaction();

            try
            {
                var sql = "UPDATE BankAccount SET Balance = Balance - @Amount WHERE AccountNumber = @From";
                var cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Amount", Amount);
                cmd.Parameters.AddWithValue("@From", from);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();

                sql = "UPDATE BankAccount SET Balance = Balance + @Amount WHERE AccountNumber = @To";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Amount", Amount);
                cmd.Parameters.AddWithValue("@To", To);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                
                sql = "INSERT INTO Transactions (SenderId, ReceiverId, Amount, SentAt) VALUES (@From, @To, @Amount, @SentAt)";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@From", fromAccount!.Id);
                cmd.Parameters.AddWithValue("@To", toAccount!.Id);
                cmd.Parameters.AddWithValue("@Amount", Amount);
                cmd.Parameters.AddWithValue("@SentAt", DateTime.Now);
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

    public IList<RolesModel> GetRolesById(int uid)
    {
        var roles = new List<RolesModel>();
        
        var sql = "SELECT [UserRoles].Id, [UserRoles].RoleName, [UserRoles].UserId FROM [UserRoles] WHERE [UserRoles].UserId = @Id";
        var cmd = new SqlCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@Id", uid);
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            roles.Add(new RolesModel
            {
                Id = reader.GetInt32(0),
                RoleName = reader.GetString(1),
                UserId = reader.GetInt32(2)
            });
        }

        return roles;
    }

    public IList<BankAccountModel> GetAllBankAccounts()
    {
        var accounts = new List<BankAccountModel>();
        
        var sql = "SELECT Id, AccountNumber, Balance, UserId FROM BankAccount";
        var cmd = new SqlCommand(sql, _connection);
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            accounts.Add(new BankAccountModel
            {
                Id = reader.GetInt32(0),
                AccountNumber = reader.GetString(1),
                Balance = reader.GetDecimal(2),
                UserId = reader.GetInt32(3)
            });
        }

        var users = GetUsers();
        
        foreach (var account in accounts)
        {
            account.User = users.First(u => u.Id == account.UserId);
        }

        return accounts;
    }

    public void AddFunds(Guid guid, decimal amount)
    {
        var sql = "UPDATE BankAccount SET Balance = Balance + @Amount WHERE AccountNumber = @AccountNumber";
        var cmd = new SqlCommand(sql, _connection);
        cmd.Parameters.AddWithValue("@Amount", amount);
        cmd.Parameters.AddWithValue("@AccountNumber", guid);
        cmd.ExecuteNonQuery();

    }

    public IList<TransactionModel> GetTransactions()
    {
        var sql = "SELECT Id, SenderId, ReceiverId, Amount, SentAt FROM Transactions";
        var cmd = new SqlCommand(sql, _connection);
        
        var transactions = new List<TransactionModel>();
        
        var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            transactions.Add(new TransactionModel
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                SenderId = reader.GetInt32(reader.GetOrdinal("SenderId")),
                ReceiverId = reader.GetInt32(reader.GetOrdinal("ReceiverId")),
                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                SentAt = reader.GetDateTime(reader.GetOrdinal("SentAt"))
            });
        }
        
        reader.Close();

        foreach (var transaction in transactions)
        {
            transaction.Sender = GetBankAccountByAccountId(transaction.SenderId);
            transaction.Receiver = GetBankAccountByAccountId(transaction.ReceiverId);
        }
        
        return transactions;
    }

    public IList<TransactionModel> GetTransactions(int uid)
    {
        var sql = "SELECT Id, SenderId, ReceiverId, Amount, SentAt FROM Transactions WHERE SenderId = @Id OR ReceiverId = @Id";
        var cmd = new SqlCommand(sql, _connection);
        
        cmd.Parameters.AddWithValue("@Id", uid);
        
        var transactions = new List<TransactionModel>();
        
        var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            transactions.Add(new TransactionModel
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                SenderId = reader.GetInt32(reader.GetOrdinal("SenderId")),
                ReceiverId = reader.GetInt32(reader.GetOrdinal("ReceiverId")),
                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                SentAt = reader.GetDateTime(reader.GetOrdinal("SentAt"))
            });
        }
        
        reader.Close();
        
        foreach (var transaction in transactions)
        {
            transaction.Sender = GetBankAccountByAccountId(transaction.SenderId);
            transaction.Receiver = GetBankAccountByAccountId(transaction.ReceiverId);
        }
        
        return transactions;
    }
}