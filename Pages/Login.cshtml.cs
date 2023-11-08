using System.Security.Claims;
using BankApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages
{

    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly LoginUserService _loginUserService;

        public LoginModel(ILogger<LoginModel> logger)
        {
            _logger = logger;
            _loginUserService = new LoginUserService();
        }

        /// <summary>
        /// nothing to do here
        /// </summary>
        public void OnGet()
        {
        }

        /// <summary>
        /// handles the login post request
        /// </summary>
        /// <param name="username">username from post request</param>
        /// <param name="password">sha256 hashed password from the post request</param>
        public void OnPost(
            string username,
            string password
            )
        {
            _loginUserService.RefreshData();

            _logger.LogDebug($"Username: {username}\tPassword: {password}");

            if (ValidateLogin(username, password))
            {
                SignInUser(username, GetUidForUser(username, password));
                Response.Redirect("/index");
            }
            else
            {
                Response.Redirect("/login");
            }
        }

        /// <summary>
        /// Queries the database for the specified user
        /// </summary>
        /// <param name="username">queried username</param>
        /// <param name="password">queried hashed by sha256 password</param>
        /// <returns></returns>
        public bool ValidateLogin(string username, string password)
        {
            bool valid = false;

            foreach (var User in _loginUserService.GetUsers())
            {
                if (User.Username == username)
                {
                    if (User.PasswordHash == password)
                    {
                        valid = true;
                        break;
                    }
                }
            }

            return valid;
        }

        /// <summary>
        /// Queries the database for the User Id
        /// </summary>
        /// <param name="username">queried username</param>
        /// <param name="password">queried hashed by sha256 password</param>
        /// <returns></returns>
        public string GetUidForUser(string username, string password)
        {
            foreach (var Users in _loginUserService.GetUsers())
            {
                if (Users.Username == username && Users.PasswordHash == password)
                {
                    return Users.Id.ToString();
                }
            }

            return null;
        }


        /// <summary>
        /// Signs in the user with the specified username and uid
        /// </summary>
        /// <param name="username">Sign in username</param>
        /// <param name="uid">Sign in User Id</param>
        public void SignInUser(string username, string uid)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("UserId", uid)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }
    }
}
