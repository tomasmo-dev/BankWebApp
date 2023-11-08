using BankApp.Data;
using MySql.Data.MySqlClient;

namespace BankApp.Services
{
    public class DatabaseService : IDisposable
    {
        private readonly MySqlConnection _connection;
        public bool IsOpened { get; private set; }

        /// <summary>
        /// Creates Connection String with given parameters
        /// and prepares the connection.
        /// </summary>
        /// <param name="address">the address of the database</param>
        /// <param name="port">port of the database</param>
        /// <param name="uid">username for the user used for connection</param>
        /// <param name="pwd">password for the user</param>
        /// <param name="database">"default" selected database</param>
        public DatabaseService(string address, uint port, string uid, string pwd, string database)
        {
            MySqlConnectionStringBuilder builder = new();
            builder.Server = address;
            builder.Port = port;
            builder.UserID = uid;
            builder.Password = pwd;
            builder.Database = database;

            _connection = new MySqlConnection(builder.ToString());
        }

        /// <summary>
        /// Creates connection with already prepared connection string.
        /// </summary>
        /// <param name="ConnectionString">valid prepared connection string</param>
        public DatabaseService(string ConnectionString)
        {
            _connection = new MySqlConnection(ConnectionString);
        }

        public void Dispose()
        {
            if (IsOpened) _connection.Close();

            _connection.Dispose();
        }

        public void Open()
        {
            _connection.Open();
            IsOpened = true;
        }

        public void Close()
        {
            _connection.Close();
            IsOpened = false;
        }

        public IList<LoginUserModel> GetUsers()
        {
            var cmd = _connection.CreateCommand();
            string query = "SELECT `Id`, `Username`, `PasswordHash`, `CreatedAt`, `ContactId`, `AddressId` FROM `Users`";
            cmd.CommandText = query;

            var reader = cmd.ExecuteReader();

            IList<LoginUserModel> users = new List<LoginUserModel>();

            while (reader.Read())
            {
                LoginUserModel user = new();

                user.Id = reader.GetInt32(0);
                user.Username = reader.GetString(1);
                user.PasswordHash = reader.GetString(2);
                user.CreatedAt = reader.GetDateTime(3);
                user.ContactId = reader.GetInt32(4);
                user.AddressId = reader.GetInt32(5);
                users.Add(user);
            }

            reader.Close();

            return users;
        }
    }
}
