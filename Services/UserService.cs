using BankWebApp.Models;
using BankWebApp.Tools;

namespace BankWebApp.Services
{
    /// <summary>
    /// Service class for managing users.
    /// </summary>
    public class UserService
    {
        private readonly DatabaseService _databaseService;

        private DateTime _lastUpdateAt;

        private IList<UserModel>? _users = null;
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
        
        public UserModel? GetUserById(int id)
        {
            return _users?.FirstOrDefault(user => user.Id == id);
        }
        
        public UserModel? GetUserByUsername(string username)
        {
            return _users?.FirstOrDefault(user => user.Username == username);
        }

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
        /// Gets the bank accounts of a user by their id.
        /// </summary>
        /// <param name="uid">The User Id of the owner</param>
        /// <returns>A List of BankAccounts that the user owns</returns>
        public IList<BankAccountModel> GetBankAccountsById(int uid)
        {
            return _databaseService.GetBankAccountById(uid);
        }
        
        /// <summary>
        /// Gets the bank account of a user by its account number.
        /// </summary>
        /// <param name="id">the bank account number to search for ( unique )</param>
        /// <returns>the specific bank account</returns>
        public BankAccountModel? GetBankAccountsById(string id)
        {
            return _databaseService.GetBankAccountById(id);
        }
        
        public IList<BankAccountModel> GetAllBankAccounts()
        {
            return _databaseService.GetAllBankAccounts();
        }
        
        public IList<RolesModel> GetRolesById(int uid)
        {
            return _databaseService.GetRolesById(uid);
        }
        
        public IList<TransactionModel> GetAllTransactions()
        {
            return _databaseService.GetTransactions();
        }
        
        public IList<TransactionModel> GetTransactionsByAccountId(int accountId)
        {
            return _databaseService.GetTransactions(accountId);
        }
        
    }
}