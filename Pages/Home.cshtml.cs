using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Policy;


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

        public List<Tweet> Tweet { get; set; } = null!;

        [BindProperty]
        public User? CurrentUser { get; set; }

        public HomeModel(SignInManager<User> signInManager, ILogger<HomeModel> logger, TwitterCloneDbContext context, UserManager<User> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        // private List<ShowTrendModel> getTrend(User currentUser)
        // {
        //     var randomTweets = context.Tweets
        // .AsEnumerable() // Switch to client-side evaluation
        // .Where(t => t.Author.Id != currentUser.Id)
        // .OrderBy(x => Guid.NewGuid()) // Random order
        // .Take(5) // Take 3 tweets
        // .ToList();


        // foreach (var tweet in randomTweets)
        //     {
        //         Count = context.Tweets.Where(t => t.ParentTweet.Id == tweet.Id).Count();
        //         Hashtag = tweet.Hashtag; 
        //     }

        //     return res;
        // }

        public class ShowTrendModel
        {
            public string? Hashtag { get; set; }
            public int Count { get; set; }
        }

        public ShowTrendModel? ShowTrend { get; set; }


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

                        var followSuggestionRes = User.Except(FollowedUser).ToList();
                        var followSuggestion = followSuggestionRes.Where(u => u.Id != currentUser.Id).ToList();
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

