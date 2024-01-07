using System.Security.Claims;
using BankWebApp.Models;
using Microsoft.AspNetCore.Authentication;

namespace BankWebApp.Services
{
    /// <summary>
    /// This class is responsible for managing user sign in and sign out operations.
    /// </summary>
    public class MySignInManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySignInManager"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="userService">The user service.</param>
        public MySignInManager(IHttpContextAccessor httpContextAccessor, UserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        /// <summary>
        /// Signs in the specified user.
        /// </summary>
        /// <param name="user">The user to sign in.</param>
        /// <param name="isPersistent">if set to <c>true</c> the sign in is persistent.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// The user model should already be validated before calling this method.
        /// </remarks>
        public async Task SignInAsync(UserModel user, bool isPersistent = false)
        {
            var roles = _userService.GetRolesById(user.Id);

            // Create a claims identity
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
            };

            claims.AddRange(roles.Select(
                role => new Claim(ClaimTypes.Role, role.RoleName)
                ));

            var claimsIdentity = new ClaimsIdentity(claims, "custom");

            // Create a claims principal
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Sign in the user
            await _httpContextAccessor.HttpContext.SignInAsync("custom", claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30) // Set expiration as needed
            });

        }

        /// <summary>
        /// Signs out the current user.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SignOutAsync()
        {
            // Sign out the user
            await _httpContextAccessor.HttpContext.SignOutAsync("custom");
        }
    }
}