using System.Security.Claims;
using BankWebApp.Models;
using Microsoft.AspNetCore.Authentication;

namespace BankWebApp.Services;

public class MySignInManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserService _userService;

    public MySignInManager(IHttpContextAccessor httpContextAccessor, UserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public async Task SignInAsync(UserModel user, bool isPersistent = false)
    {
        // The user model should already be validated before calling this method
        
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

    public async Task SignOutAsync()
    {
        // Sign out the user
        await _httpContextAccessor.HttpContext.SignOutAsync("custom");
    }
}
