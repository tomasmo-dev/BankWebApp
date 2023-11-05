using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages
{
    public class LoginModel : PageModel
    {
        public void OnGet()
        {
        }

        public void OnPost(
            string username,
            string password
            )
        {
            Debug.WriteLine($"{username};{password}");
        }
    }
}
