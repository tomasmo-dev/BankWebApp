using BankWebApp.env;
using Microsoft.Data.SqlClient;

namespace BankWebApp.Services;

public partial class DatabaseService : IDisposable
{
    private readonly string _connectionString;

    private SqlConnection _connection;
    
    public DatabaseService()
    {
        _connectionString = Envs.ConnectionString;
        _connection = new SqlConnection(_connectionString);
        
        Open();
    }

    private void Open()
    {
        _connection.Open();
    }
    
    private void Close()
    {
        _connection.Close();
    }

    public void Dispose()
    {
        Close();
    }
}