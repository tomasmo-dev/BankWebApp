using Microsoft.AspNetCore.Mvc;

namespace BankWebApp.Components;

public class NavbarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}