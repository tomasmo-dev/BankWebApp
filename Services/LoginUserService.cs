using BankApp.Data;
using BankApp.env;

namespace BankApp.Services
{
    /// <summary>
    /// Service that handles fetching user data from the database
    /// </summary>
    public class LoginUserService: IDisposable
    {
        // interval for refreshing the data from the database
        private readonly TimeSpan RefreshInterval = TimeSpan.FromMinutes(1);

        // underlying database service
        private readonly DatabaseService _databaseService;

        // list of users that gets refreshed every RefreshInterval
        private IList<LoginUserModel> LoginUsers { get; set; }
        // last refresh of the data
        private DateTime LastRefreshed { get; set; }

        /// <summary>
        /// automatically creates DatabaseService with env variables and opens the connection
        /// </summary>
        public LoginUserService()
        {
            _databaseService = new DatabaseService(env_vars.address, env_vars.port, env_vars.uid, env_vars.pwd, env_vars.default_database);
            _databaseService.Open();

            LoginUsers = new List<LoginUserModel>();
        }

        /// <summary>
        /// closes the connection from the underlying database service and disposes of the object
        /// </summary>
        public void Dispose()
        {
            _databaseService.Dispose();
        }

        public void RefreshData()
        {
            if (!_databaseService.IsOpened) throw new Exception("Unopened connection to the database");

            LoginUsers = _databaseService.GetUsers();
            LastRefreshed = DateTime.Now;
        }

        /// <summary>
        /// returns the list of users
        /// </summary>
        /// <returns>Enumerable list of all users from the DB</returns>
        public IEnumerable<LoginUserModel> GetUsers()
        {
            if (DateTime.Now - LastRefreshed > RefreshInterval)
            {
                RefreshData();
            }

            return LoginUsers;
        }
    }
}
