using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace TwitterClone.Pages
{
    public class FollowSuggestionModel : PageModel
    {

        private readonly UserManager<User> userManager;
        private readonly ILogger<HomeModel> logger;
        private readonly TwitterCloneDbContext context;

        public List<User> FollowedUser { get; set; } = null!;

        public List<User> FollowSuggestion { get; set; } = null!;

        [BindProperty]
        public User? CurrentUser { get; set; }

        public int ShowDescription { get; set; } = 1;  // 0: hide, 1: show

        public FollowSuggestionModel(UserManager<User> userManager, ILogger<HomeModel> logger, TwitterCloneDbContext context)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.context = context;
        }
        public async Task OnGetAsync()
        {
            // get all users
            var allUsers = userManager.Users.ToList();
            // get current user
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser != null)
            {
                CurrentUser = currentUser;
                Console.WriteLine("Current User: " + CurrentUser.UserName);
                // get all users that current user is following
                var followedUser = await context.Follows.Where(f => f.User.Id == currentUser.Id).Select(f => f.Author).ToListAsync();

                if (followedUser.Count > 0)
                {  // if current user is following someone, show users that current user is not following
                    FollowedUser = followedUser;
                    var excludeFollowed = allUsers.Except(FollowedUser).ToList();
                    FollowSuggestion = excludeFollowed.Where(user => user.Id != currentUser.Id).ToList();
                }
                else
                {  // if current user is not following anyone, show all users
                    FollowSuggestion = allUsers.Where(user => user.Id != currentUser.Id).ToList();
                }
            }
        }
    }
}
