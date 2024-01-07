using BankWebApp.Models;
using BankWebApp.Services;
using BankWebApp.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankWebApp.Controllers;

/// <summary>
/// The AccountController class is responsible for handling requests related to the user's bank account.
/// It includes actions for displaying the account index, transferring funds, adding funds (admin only), listing users (admin only), showing user details (admin only), deleting a user (admin only), and viewing transaction history.
/// </summary>
[Authorize]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserService _userService;
    private readonly TransferService _transferService;

    /// <summary>
    /// Initializes a new instance of the AccountController class.
    /// </summary>
    /// <param name="logger">The logger used to log information about program execution.</param>
    /// <param name="userService">The service used to interact with user data.</param>
    /// <param name="transferService">The service used to handle money transfers.</param>
    public AccountController(ILogger<AccountController> logger, UserService userService,
                            TransferService transferService)
    {
        _logger = logger;
        _userService = userService;
        _transferService = transferService;
    }

    /// <summary>
    /// Displays the account index page.
    /// </summary>
    /// <returns>The account index view.</returns>
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

    /// <summary>
    /// Displays the transfer page.
    /// </summary>
    /// <param name="success">Indicates whether the last transfer was successful.</param>
    /// <param name="reason">The reason for the last transfer's failure, if it failed.</param>
    /// <returns>The transfer view.</returns>
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

    /// <summary>
    /// Handles a POST request to transfer funds.
    /// </summary>
    /// <param name="model">The data from the transfer form.</param>
    /// <returns>A redirect to the transfer page, with success and reason parameters.</returns>
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

    /// <summary>
    /// Displays the add funds page (admin only).
    /// </summary>
    /// <param name="success">Indicates whether the last add funds operation was successful.</param>
    /// <param name="reason">The reason for the last add funds operation's failure, if it failed.</param>
    /// <returns>The add funds view.</returns>
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

    /// <summary>
    /// Handles a POST request to add funds to an account (admin only).
    /// </summary>
    /// <param name="model">The data from the add funds form.</param>
    /// <returns>A redirect to the add funds page, with success and reason parameters.</returns>
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

    /// <summary>
    /// Displays the list users page (admin only).
    /// </summary>
    /// <returns>The list users view.</returns>
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

    /// <summary>
    /// Displays the details of a user (admin only).
    /// </summary>
    /// <param name="id">The ID of the user to show.</param>
    /// <returns>The show view.</returns>
    [Authorize(Roles = "Admin")]
    public IActionResult Show(string id)
    {
        if (!int.TryParse(id, out _))
        {
            return Problem("Invalid user id. (must be of type int)");
        }

        var user = _userService.GetUserById(int.Parse(id));
        var bankAccounts = _userService.GetBankAccountsById(user!.Id);

        var model = new ListUsersViewModel()
        {
            UserModel = user,
            BankAccounts = bankAccounts
        };


        return View(model);
    }

    /// <summary>
    /// Deletes a user (admin only).
    /// </summary>
    [Authorize(Roles = "Admin")]
    public IActionResult Delete()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Displays the transaction history page.
    /// </summary>
    /// <returns>The history view.</returns>
    public IActionResult History()
    {
        AccountHistoryModel model = new();

        var myId = (User.Claims.ToArray()[1].Value).ToInt32();

        var bankAccounts = _userService.GetBankAccountsById(myId);

        // get all transactions from all bank accounts
        var transactions = bankAccounts.ToList().SelectMany(bankAccount =>
            _userService.GetTransactionsByAccountId(bankAccount.Id)).ToList();

        model.Transactions = transactions;

        return View(model);
    }
}