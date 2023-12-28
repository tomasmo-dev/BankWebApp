using BankWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BankWebApp.Services;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// HomeController class that inherits from Controller.
/// This class is responsible for handling the requests related to the Home page of the application.
/// </summary>
namespace BankWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserService _userService;
        private readonly MySignInManager _signInManager;

        /// <summary>
        /// HomeController constructor.
        /// Initializes a new instance of the HomeController class.
        /// </summary>
        /// <param name="logger">An instance of ILogger interface to handle logging.</param>
        /// <param name="userService">An instance of UserService to handle user related operations.</param>
        public HomeController(ILogger<HomeController> logger, UserService userService, MySignInManager signInManager)
        {
            _logger = logger;
            _userService = userService;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Handles the GET request for the Index view.
        /// </summary>
        /// <returns>The Index view.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Handles the GET request for the Privacy view.
        /// </summary>
        /// <returns>The Privacy view.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Handles the GET request for the Login view.
        /// </summary>
        /// <returns>The Login view.</returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Handles the POST request for the Login view.
        /// </summary>
        /// <param name="loginModel">The login details provided by the user.</param>
        /// <returns>The Login view if the model state is invalid, otherwise redirects to the appropriate view based on the login details.</returns>
        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var Username = loginModel.Username;
                var Password = loginModel.Password;

                if (string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                {
                    _logger.LogDebug($"Login failed for user {Username} (empty credentials)");
                    return RedirectToAction();
                }
                
                var user = _userService.GetUserByUsername(Username);
                if (user == null)
                {
                    _logger.LogDebug($"Login failed for user {Username} (account not found)");
                    return RedirectToAction();
                }

                if (!Tools.PasswordHashes.VerifyPassword(Password, user.PasswordHash))
                {
                    _logger.LogDebug($"Login failed for user {Username} (Password doesnt match)");
                    return RedirectToAction();
                }

                _signInManager.SignInAsync(user, loginModel.RememberMe);

                _logger.LogDebug($"Login successful for user {Username}");
                
                return RedirectToAction("Index", "Account");

            }
            return View();
        }

        public IActionResult Logout()
        {
            var signedIn = User.Identity!.IsAuthenticated;
            if (!signedIn)
            {
                return RedirectToAction("Login");
            }
            
            _signInManager.SignOutAsync().GetAwaiter().GetResult(); //wait for sign out before redirecting
            return RedirectToAction("Login");
        }
        
        /// <summary>
        /// Handles the GET request for the Error view.
        /// </summary>
        /// <returns>The Error view along with the request id.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        public IActionResult AccessDenied()
        {
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Register()
        {
            return View();
        }
        
        // TODO: say why model invalid
        [HttpPost]
        public IActionResult Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var resp = _userService.RegisterUser(registerModel);
                
                if (resp.success)
                {
                    _logger.LogDebug($"Registered user {registerModel.Username}");

                    registerModel.Success = true;
                    
                    return RedirectToAction("Login");
                }
                else
                {
                    _logger.LogDebug($"Failed to register user {registerModel.Username}: {resp.reason}");
                    ModelState.AddModelError("Username", resp.reason);
                    
                    registerModel.Success = false;
                    registerModel.Reason = resp.reason;
                    
                    return View(registerModel);
                }
            }

            registerModel.Success = false;
            registerModel.Reason = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));;
            
            return View(registerModel);
        }
    }
}