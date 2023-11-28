using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using TwitterClone.Models;
using TwitterClone.Data;


namespace TwitterClone.Pages
{
    public class UserModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly ILogger<UserModel> logger;
        private readonly TwitterCloneDbContext db;

        [BindProperty(SupportsGet = true)]
        public string Username { get; set; }

        public User? Profile { get; set; }

        public UserModel(UserManager<User> userManager, ILogger<UserModel> logger, TwitterCloneDbContext db)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.db = db;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = db.Users.Where(u => u.UserName == Username).FirstOrDefault();

            if (user != null)
            {
                Profile = user;
                return Page();

            }
            else
            {
                return RedirectToPage("NotFound");
            }
        }
    }
}
