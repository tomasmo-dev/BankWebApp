using System.Security.Claims;
using BankWebApp.Models;
using Microsoft.AspNetCore.Authentication;

namespace BankWebApp.Services;

public class MySignInManager
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public MySignInManager(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task SignInAsync(UserModel user, bool isPersistent = false)
    {
        // The user model should already be validated before calling this method
        
            // Create a claims identity
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "custom");

            // Create a claims principal
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Sign in the user
            await httpContextAccessor.HttpContext.SignInAsync("custom", claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30) // Set expiration as needed
            });
        
    }

    public async Task SignOutAsync()
    {
        // Sign out the user
        await httpContextAccessor.HttpContext.SignOutAsync("custom");
    }
}
