using BankWebApp.Models;
using BankWebApp.Services;
using BankWebApp.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankWebApp.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserService _userService;
    
    public AccountController(ILogger<AccountController> logger, UserService userService)
    {
        _logger = logger;
        _userService = userService;
    }
    
    // GET
    public IActionResult Index()
    {
        // users claims
        var claims = User.Claims.ToArray();
        
        // extract information from claims (username not needed)
        //string username = claims[0].Value;
        int id = claims[1].Value.ToInt32();
        
        var user = _userService.GetUserById(id)!;

        var model = new AccountIndexModel()
        {
            SignedInUser = user
        };
        
        return View(model);
    }
}