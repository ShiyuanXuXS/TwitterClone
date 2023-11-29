using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;


namespace TwitterClone.Pages.Shared.Components.WhatsHappening
{
    public class WhatsHappeningViewComponent : ViewComponent
    {

        private readonly ILogger<HomeModel> logger;
        private readonly TwitterCloneDbContext context;
        private readonly UserManager<User> userManager;
        public class ShowTrendModel
        {
            public List<string>? Hashtag { get; set; }
            public int CountLikes { get; set; }

            public int CountRetweets { get; set; }
        }

        public WhatsHappeningViewComponent(ILogger<HomeModel> logger, TwitterCloneDbContext context, UserManager<User> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
        }

        public List<ShowTrendModel>? ShowTrend { get; set; }

        private List<ShowTrendModel> getTrend(User currentUser)
        {
            var randomTweets = context.Tweets
        .AsEnumerable() // Switch to client-side evaluation
        .Where(t => t.Author.Id != currentUser.Id)
        .OrderBy(x => Guid.NewGuid()) // Random order
        .Take(5)
        .ToList();

            var trendList = new List<ShowTrendModel>();
            foreach (var tweet in randomTweets)
            {
                // count retweets for this tweet
                int countRetweets = context.Tweets.Count(t => t.ParentTweet.Id == tweet.Id);
                // count likes for this tweet
                int countLikes = context.Likes.Count(t => t.Tweet.Id == tweet.Id);
                var hashtag = Regex.Matches(tweet.Body, @"#<a>(.*?)</a>")
            .Cast<Match>()
            .Select(match => match.Groups[1].Value)
            .ToList();
                Console.WriteLine("Found Hashtags: " + hashtag.Count);

                Console.WriteLine("Hashtags: " + string.Join(", ", hashtag));
                if (hashtag.Count > 0)
                {
                    trendList.Add(new ShowTrendModel { Hashtag = hashtag, CountLikes = countLikes, CountRetweets = countRetweets });
                }
                else
                {
                    trendList.Add(new ShowTrendModel { Hashtag = new List<string> { "new", "trend" }, CountLikes = countLikes, CountRetweets = countRetweets });
                }

            }
            return trendList;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser != null)
            {
                ShowTrend = getTrend(currentUser);
            }
            return View(ShowTrend);

        }
    }
}