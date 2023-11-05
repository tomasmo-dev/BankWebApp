using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;

namespace BankWebsite.Data
{
    public  class DatabaseService : IDisposable
    {
        private const string connectionString = "Server=localhost,6661;Database=Bank;User Id=SA;Password=Heslo1234.;"; // not Ideal but works for this demo
        // using this user is not ideal too

        private SqlConnection _connection;
        private bool _opened = false;

        public DatabaseService()
        {
            _connection = new SqlConnection(connectionString);
            try 
            { 
                _connection.Open(); 
                _opened = true;
            }
            catch(SqlException exception) 
            {
                throw new Exception("Failed to open connection with SQL server", exception);
            }
        }

        ~DatabaseService()
        {
            DisposeConnection();
        }

        public void Dispose()
        {
            DisposeConnection();
        }

        private void DisposeConnection()
        {
            if (_opened)
            {
                _connection.Close();
            }

            _connection.Dispose();
        }
    }
}
