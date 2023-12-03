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
        private readonly UserManager<User> userManager;
        private readonly ILogger<LogoutModel> logger;

        public LogoutModel(SignInManager<User> signInManager, ILogger<LogoutModel> logger, UserManager<User> userManager)
        {
            this.signInManager = signInManager;
            this.logger = logger;
            this.userManager = userManager;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var currentUser = await userManager.GetUserAsync(User);
            await signInManager.SignOutAsync();
            logger.LogInformation($"User {currentUser.UserName} logged out.");
            TempData["FlashMessage"] = $"User {currentUser.UserName} logged out.";
            return RedirectToPage("Index");
        }
    }
}
