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
    private readonly TransferService _transferService;
    
    public AccountController(ILogger<AccountController> logger, UserService userService,
                            TransferService transferService)
    {
        _logger = logger;
        _userService = userService;
        _transferService = transferService;
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
        var bankAccounts = _userService.GetBankAccountsById(user.Id);

        var model = new AccountIndexModel()
        {
            SignedInUser = user,
            BankAccounts = bankAccounts
        };
        
        return View(model);
    }

    public IActionResult Transfer(bool? success = null, string? reason = null)
    {
        var model = TransferViewModel.Empty;
        var userId = User.Claims.ToArray()[1].Value.ToInt32();
        
        model.Success = success;
        model.Reason = reason;

        var bankAccounts = _userService.GetBankAccountsById(userId);
        model.BankAccounts = bankAccounts;
        
        return View(model);
    }
    
    [HttpPost]
    public IActionResult Transfer(TransferViewModel model)
    {
        var success = true;
        var reason = "";
        
        // check if from account is valid and the user owns it
        var fromAccount = _userService.GetBankAccountsById(model.FromAccountId);
        
        if (fromAccount == null)
        {
            success = false;
            reason = "Invalid account to send from.";
            return RedirectToAction("Transfer", new { Success = success, Reason = reason });
        }
        
        var userId = User.Claims.ToArray()[1].Value.ToInt32();
        var user = _userService.GetUserById(userId)!;
        var userOwnsAccount = fromAccount.UserId == user.Id;
            
        if (!userOwnsAccount)
        {
            success = false;
            reason = "You do not own this account.";
            
            return RedirectToAction("Transfer", new { Success = success, Reason = reason });
        }
        
        // now check if to account is valid
        
        var toAccount = _userService.GetBankAccountsById(model.ToAccountId);
        
        if (toAccount == null)
        {
            success = false;
            reason = "Invalid account to send to.";
            return RedirectToAction("Transfer", new { Success = success, Reason = reason });
        }
        
        // check if amount is valid
        
        if (model.Amount <= 0)
        {
            success = false;
            reason = "Invalid amount (cannot be less than 0).";
            return RedirectToAction("Transfer", new { Success = success, Reason = reason });
        }
        
        // check if from account has enough money
        
        if (fromAccount.Balance <= model.Amount)
        {
            success = false;
            reason = "Not enough money in account.";
            return RedirectToAction("Transfer", new { Success = success, Reason = reason });
        }
        
        // transfer money
        
        var transferCheck = _transferService.TransferMoney(fromAccount.AccountNumber, toAccount.AccountNumber, model.Amount);
        
        
        return RedirectToAction("Transfer", new { Success = success, Reason = reason });
    }

    [Authorize(Roles = "Admin")]
    public IActionResult AddFunds(bool? success = null, string? reason = null)
    {
        var model = new AddFundsViewModel()
        {
            BankAccounts = _userService.GetAllBankAccounts(),
            Success = success,
            Reason = reason
        };
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult AddFunds(AddFundsViewModel model)
    {
        bool success = true;
        string reason = "";
        
        // check if such account exists
        var bankAccount = _userService.GetBankAccountsById(model.SelectedBankAccountNumber);
        
        if (bankAccount == null)
        {
            success = false;
            reason = "Invalid account.";
            return RedirectToAction("AddFunds", new { Success = success, Reason = reason });
        }
        
        // check if amount is valid
        
        if (model.Amount <= 0)
        {
            success = false;
            reason = "Invalid amount (cannot be less than 0).";
            return RedirectToAction("AddFunds", new { Success = success, Reason = reason });
        }
        
        // add funds
        _transferService.PrintMoney(model.SelectedBankAccountNumber, model.Amount);
        
        return RedirectToAction("AddFunds", new { Success = success, Reason = reason });
    }
    
    [Authorize(Roles = "Admin")]
    public IActionResult ListUsers()
    {
        var model = new List<ListUsersViewModel>();
        
        var users = _userService.GetUsers();

        foreach (var user in users)
        {
            ListUsersViewModel userViewModel = new()
            {
                UserModel = user,
                BankAccounts = _userService.GetBankAccountsById(user.Id)
            };
            
            model.Add(userViewModel);
        }
        
        return View(model);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Show(string id)
    {
        return View();
    }
}