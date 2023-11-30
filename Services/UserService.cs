using BankWebApp.Models;

namespace BankWebApp.Services;

public class UserService
{
    private readonly DatabaseService _databaseService;

    private DateTime _lastUpdateAt;
    
    private IList<UserModel> _users;
    private TimeSpan _cacheDuration = TimeSpan.FromMinutes(1);
    
    public UserService()
    {
        _databaseService = new DatabaseService();
    }
    
    public IList<UserModel> GetUsers()
    {
        if (_users == null || (_lastUpdateAt + _cacheDuration) < DateTime.Now)
        {
            _users = _databaseService.GetUsers();
            _lastUpdateAt = DateTime.Now;
        }

        return _users;
    }
}