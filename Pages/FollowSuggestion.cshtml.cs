using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;



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
        public int IndexDropDown { get; set; } = 1;

        public int IndexShowTweet { get; set; } = 0; // 0: hide, 1: show

        public FollowSuggestionModel(UserManager<User> userManager, ILogger<HomeModel> logger, TwitterCloneDbContext context)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.context = context;
        }

        public List<ShowTrendModel>? ShowTrend { get; set; }

        private List<ShowTrendModel> getTrend(User currentUser)
        {
            var randomTweets = context.Tweets
        .AsEnumerable() // Switch to client-side evaluation
        .Where(t => t.Author.Id != currentUser.Id)
        .ToList();

            var trendList = new List<ShowTrendModel>();
            foreach (var tweet in randomTweets)
            {
                // count retweets for this tweet
                int countRetweets = context.Tweets.Count(t => t.ParentTweet.Id == tweet.Id);
                // count likes for this tweet
                int countLikes = context.Likes.Count(t => t.Tweet.Id == tweet.Id);
                var hashtag = Regex.Matches(tweet.Body, @"<a[^>]*?>(.*?)</a>")
            .Cast<Match>()
            .Select(match => match.Groups[1].Value)
            .ToList();
                if (hashtag.Count > 0)
                {
                    trendList.Add(new ShowTrendModel { Hashtag = hashtag, CountLikes = countLikes, CountRetweets = countRetweets, Id = tweet.Id });
                }
                else
                {
                    trendList.Add(new ShowTrendModel { Hashtag = new List<string> { "new", "trend" }, CountLikes = countLikes, CountRetweets = countRetweets, Id = tweet.Id });
                }
            }
            return trendList;
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
                ShowTrend = getTrend(CurrentUser);
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
