using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;


namespace TwitterClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly TwitterCloneDbContext context;
        private User CurrentUser { get; set; } = null!;

        public class ShowTrendModel
        {
            public int Id { get; set; }
            public List<string>? Hashtag { get; set; }
            public int CountLikes { get; set; }
            public int CountRetweets { get; set; }
        }
        public TweetController(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, TwitterCloneDbContext context)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
        }
        public List<ShowTrendModel>? ShowTrend { get; set; }

        private List<ShowTrendModel> getTrend(User currentUser)
        {
            var randomTweets = context.Tweets
        .Where(t => t.Author != null && t.Author.Id != currentUser.Id)
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

        [HttpGet("updateTrend")]
        public async Task<PartialViewResult> _PartialWhatsHappening(int tweetId)
        {
            try
            {
                int tweetIdToRemove = tweetId;
                Console.WriteLine("tweetId: " + tweetIdToRemove);
                Console.WriteLine("called!!!!");
                var currentUser = await userManager.GetUserAsync(HttpContext.User);
                Console.WriteLine("no error here.....");
                var updatedTrendData = getTrend(currentUser).Where(item => item.Id != tweetIdToRemove).ToList();
                Console.WriteLine("no error here");
                return PartialView(updatedTrendData);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                throw; // Rethrow the exception
            }

        }
    }
}