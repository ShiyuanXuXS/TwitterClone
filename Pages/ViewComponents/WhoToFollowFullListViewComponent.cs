using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.EntityFrameworkCore;


namespace TwitterClone.Pages.ViewComponents
{
    public class WhoToFollowFullListViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<MainModel> _logger;
        private readonly TwitterCloneDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public new List<User> User { get; set; } = null!;
        public List<User> FollowedUser { get; set; } = null!;

        public List<User> FollowSuggestion { get; set; } = null!;

        [BindProperty]
        public User? CurrentUser { get; set; }

        public WhoToFollowFullListViewComponent(UserManager<User> userManager, ILogger<MainModel> logger, TwitterCloneDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // get all users
            User = await _userManager.Users.ToListAsync();
            // get current user
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser != null)
            {
                CurrentUser = currentUser;
                Console.WriteLine("Current User: " + CurrentUser.UserName);
                // get all users that current user is following
                var followedUser = await _context.Follows.Where(f => f.User.Id == currentUser.Id).Select(f => f.Author).ToListAsync();
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
            return View(FollowSuggestion);
        }

    }
}