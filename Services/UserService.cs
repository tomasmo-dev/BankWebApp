using BankWebApp.Models;
using BankWebApp.Tools;

namespace BankWebApp.Services
{
    /// <summary>
    /// Service class for managing users.
    /// </summary>
    public class UserService
    {
        // Database service instance for database operations
        private readonly DatabaseService _databaseService;

        // The time when the cache was last updated
        private DateTime _lastUpdateAt;

        // Cache for storing user data
        private IList<UserModel>? _users = null;

        // Duration for which the cache is valid
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Constructor for UserService. Initializes a new instance of the DatabaseService and refreshes the cache.
        /// </summary>
        public UserService()
        {
            _databaseService = new DatabaseService();
            RefreshCache();
        }

        /// <summary>
        /// Retrieves the list of users. If the cache is null or expired, it refreshes the cache before returning the users.
        /// </summary>
        /// <returns>A list of UserModel instances.</returns>
        public IList<UserModel> GetUsers()
        {
            if (_users == null || (_lastUpdateAt + _cacheDuration) < DateTime.Now)
            {
                RefreshCache();
            }

            return _users!;
        }

        /// <summary>
        /// Refreshes the cache by retrieving the users from the database and updating the last update time.
        /// </summary>
        private void RefreshCache()
        {
            _users = _databaseService.GetUsers();
            _lastUpdateAt = DateTime.Now;
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The user with the given ID, or null if no such user exists.</returns>
        public UserModel? GetUserById(int id)
        {
            return _users?.FirstOrDefault(user => user.Id == id);
        }

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>The user with the given username, or null if no such user exists.</returns>
        public UserModel? GetUserByUsername(string username)
        {
            return _users?.FirstOrDefault(user => user.Username == username);
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="newUser">The details of the new user.</param>
        /// <returns>A tuple indicating whether the registration was successful and a reason for failure, if applicable.</returns>
        public (bool success, string reason) RegisterUser(RegisterModel newUser)
        {
            bool existCheck = _databaseService.UsernameExists(newUser.Username);
            if (existCheck)
            {
                return (false, "Username already exists");
            }

            bool passwordMatches = (newUser.Password == newUser.ConfirmPassword);
            if (!passwordMatches)
            {
                return (false, "Passwords do not match");
            }

            var NewUser = new UserModel()
            {
                // Id auto generated
                Username = newUser.Username,
                PasswordHash = newUser.Password.HashPassword(),
                Contact = new ContactModel()
                {
                    // Id auto generated
                    Email = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber
                },
                Address = new AddressModel()
                {
                    // Id auto generated
                    Street = newUser.Street,
                    City = newUser.City,
                    PostCode = newUser.PostCode,
                    Country = newUser.Country
                }
                // CreatedAt auto generated
            };

            var registerCheck = _databaseService.RegisterUser(NewUser);

            if (registerCheck)
            {
                RefreshCache();
                return (true, "");
            }
            else
            {
                return (false, "Something went wrong during the registering process");
            }
        }

        /// <summary>
        /// Retrieves the bank accounts of a user by their ID.
        /// </summary>
        /// <param name="uid">The ID of the user.</param>
        /// <returns>A list of bank accounts owned by the user.</returns>
        public IList<BankAccountModel> GetBankAccountsById(int uid)
        {
            return _databaseService.GetBankAccountById(uid);
        }

        /// <summary>
        /// Retrieves a bank account by its account number.
        /// </summary>
        /// <param name="id">The account number of the bank account.</param>
        /// <returns>The bank account with the given account number, or null if no such account exists.</returns>
        public BankAccountModel? GetBankAccountsById(string id)
        {
            return _databaseService.GetBankAccountById(id);
        }

        /// <summary>
        /// Retrieves all bank accounts.
        /// </summary>
        /// <returns>A list of all bank accounts.</returns>
        public IList<BankAccountModel> GetAllBankAccounts()
        {
            return _databaseService.GetAllBankAccounts();
        }

        /// <summary>
        /// Retrieves the roles of a user by their ID.
        /// </summary>
        /// <param name="uid">The ID of the user.</param>
        /// <returns>A list of roles assigned to the user.</returns>
        public IList<RolesModel> GetRolesById(int uid)
        {
            return _databaseService.GetRolesById(uid);
        }

        /// <summary>
        /// Retrieves all transactions.
        /// </summary>
        /// <returns>A list of all transactions.</returns>
        public IList<TransactionModel> GetAllTransactions()
        {
            return _databaseService.GetTransactions();
        }

        /// <summary>
        /// Retrieves the transactions of a bank account by its ID.
        /// </summary>
        /// <param name="accountId">The ID of the bank account.</param>
        /// <returns>A list of transactions associated with the bank account.</returns>
        public IList<TransactionModel> GetTransactionsByAccountId(int accountId)
        {
            return _databaseService.GetTransactions(accountId);
        }

    }
}