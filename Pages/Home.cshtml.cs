using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Policy;
using System.Text.RegularExpressions;



namespace TwitterClone.Pages
{
    [Authorize]
    public class HomeModel : PageModel
    {
        private readonly ILogger<HomeModel> logger;
        private readonly TwitterCloneDbContext context;
        private readonly UserManager<User> userManager;

        public List<User> FollowedUser { get; set; } = null!;
        public List<User> FollowSuggestion { get; set; } = null!;

        public List<Tweet> Tweet { get; set; } = null!;

        public int ShowDescription { get; set; } = 0;  // 0: hide, 1: show

        [BindProperty]
        public User? CurrentUser { get; set; }

        public HomeModel(ILogger<HomeModel> logger, TwitterCloneDbContext context, UserManager<User> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
        }

        // who to follow
        private List<User> getFollowSuggestion(User currentUser)
        {
            // get all users
            var allUsers = userManager.Users.ToList();
            // get all users that current user is following
            var followedUser = context.Follows.Where(f => f.User.Id == currentUser.Id).Select(f => f.Author).ToList();

            if (followedUser.Count > 0)
            {  // if current user is following someone, show users that current user is not following
                FollowedUser = followedUser;
                var excludeFollowed = allUsers.Except(FollowedUser).ToList();
                return excludeFollowed.Where(user => user.Id != currentUser.Id).ToList();
            }
            else
            {  // if current user is not following anyone, show all users
                return allUsers.Where(user => user.Id != currentUser.Id).ToList();
            }
        }

        // what's happening
        public class ShowTrendModel
        {
            public int Id { get; set; }
            public List<string>? Hashtag { get; set; }
            public int CountLikes { get; set; }
            public int CountRetweets { get; set; }
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

        public async Task<IActionResult> OnPostUpdateTrend(int tweetId)
        {
            int tweetIdToRemove = tweetId;
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var updatedTrendData = getTrend(currentUser).Where(item => item.Id != tweetIdToRemove).ToList(); ;
            // Your logic here
            return Partial("_PartialWhatsHappening", updatedTrendData);
        }

        public async Task OnGetAsync()
        {
            // get all users
            var allUsers = userManager.Users.ToList();
            // get current user
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser != null)
            {
                FollowSuggestion = getFollowSuggestion(currentUser);
                ShowTrend = getTrend(currentUser);
            }
        }





    }
}

