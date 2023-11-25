using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TwitterClone.Models;


namespace TwitterClone.Pages;

public class IndexModel : PageModel
{
    private readonly SignInManager<User> signInManager;

    private readonly ILogger<IndexModel> logger;

    public IndexModel(SignInManager<User> signInManager, ILogger<IndexModel> logger)
    {
        this.signInManager = signInManager;
        this.logger = logger;
    }

    public IActionResult OnGetAsync()
    {
        if (signInManager.IsSignedIn(User))
        {
            return RedirectToPage("Home");
        }
        else
        {
            return Page();
        }
    }
}
