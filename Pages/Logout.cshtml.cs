using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TwitterClone.Models;
using Microsoft.AspNetCore.Authorization;


namespace TwitterClone.Pages
{
    [Authorize]
    public class LogoutModel : PageModel
    {

        private readonly SignInManager<User> signInManager;
        private readonly ILogger<LogoutModel> logger;

        public LogoutModel(SignInManager<User> signInManager, ILogger<LogoutModel> logger) {
            this.signInManager = signInManager;
            this.logger = logger;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await signInManager.SignOutAsync();
            return RedirectToPage("Index");
        }
    }
}
