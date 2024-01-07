using BankWebApp.env;
using Microsoft.Data.SqlClient;

/// <summary>
/// The DatabaseService class is responsible for managing the database connection.
/// It implements the IDisposable interface to properly close the connection when it's no longer needed.
/// </summary>
namespace BankWebApp.Services
{
    public partial class DatabaseService : IDisposable
    {
        /// <summary>
        /// The connection string to the database.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// The SqlConnection object representing the database connection.
        /// </summary>
        private SqlConnection _connection;

        /// <summary>
        /// The constructor initializes a new instance of the DatabaseService class.
        /// It sets the connection string and opens the connection.
        /// </summary>
        public DatabaseService()
        {
            _connectionString = Envs.ConnectionString;
            _connection = new SqlConnection(_connectionString);

            Open();
        }

        /// <summary>
        /// The Open method opens the database connection.
        /// </summary>
        private void Open()
        {
            _connection.Open();
        }

        /// <summary>
        /// The Close method closes the database connection.
        /// </summary>
        private void Close()
        {
            _connection.Close();
        }

        /// <summary>
        /// The Dispose method is called when the DatabaseService object is being disposed.
        /// It closes the database connection.
        /// </summary>
        public void Dispose()
        {
            Close();
        }
    }
}