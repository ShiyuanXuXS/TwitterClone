using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace TwitterClone.Pages
{
    [Authorize]
    public class HomeModel : PageModel
    {
        private readonly ILogger<HomeModel> logger;
        private readonly TwitterCloneDbContext context;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public new List<User> User { get; set; } = null!;
        public List<User> FollowedUser { get; set; } = null!;
        public List<User> FollowSuggestion { get; set; } = null!;

        [BindProperty]
        public User? CurrentUser { get; set; }

        public HomeModel(SignInManager<User> signInManager, ILogger<HomeModel> logger, TwitterCloneDbContext context, UserManager<User> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public async Task OnGetAsync()
        {
            // get all users
            User = await userManager.Users.ToListAsync();
            // get current user
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser != null)
            {
                CurrentUser = currentUser;
                Console.WriteLine("Current User: " + CurrentUser.UserName);
                // get all users that current user is following
                var followedUser = await context.Follows.Where(f => f.User.Id == currentUser.Id).Select(f => f.Author).ToListAsync();
                if (User != null)
                {
                    if (followedUser.Count > 0)
                    {  // if current user is following someone, show users that current user is not following
                        FollowedUser = followedUser;
                        Console.WriteLine("Followed User: " + FollowedUser.Count);
                        var followSuggestion = User.Except(FollowedUser).ToList();
                        if (followSuggestion.Count > 0)
                        {
                            FollowSuggestion = followSuggestion;
                            Console.WriteLine("Follow Suggestion: " + FollowSuggestion.Count);
                        }
                    }
                    else
                    {  // if current user is not following anyone, show all users
                        FollowSuggestion = User;
                    }
                }
            }
            else
            {  // if current user is not logged in, show all users
                FollowSuggestion = User;
            }
        }
    }
}

