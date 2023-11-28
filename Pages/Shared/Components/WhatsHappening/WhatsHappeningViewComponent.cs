using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.EntityFrameworkCore;

namespace TwitterClone.Pages.Shared.Components.WhatsHappening
{
    public class WhatsHappeningViewComponent : ViewComponent
    {

        private readonly ILogger<HomeModel> logger;
        private readonly TwitterCloneDbContext context;
        private readonly UserManager<User> userManager;
        public class ShowTrendModel
        {
            public string? Hashtag { get; set; }
            public int Count { get; set; }
        }

        public ShowTrendModel? ShowTrend { get; set; }

        // private List<ShowTrendModel> getTrend(User currentUser)
        // {
        //     var randomTweets = context.Tweets
        // .AsEnumerable() // Switch to client-side evaluation
        // .Where(t => t.Author.Id != currentUser.Id)
        // .OrderBy(x => Guid.NewGuid()) // Random order
        // .Take(5)
        // .ToList();

        //     var trendList = new List<ShowTrendModel>();
        //     foreach (var tweet in randomTweets)
        //     {
        //         int count = context.Tweets.Count(t => t.ParentTweet.Id == tweet.Id);
        //         string hashtag = tweet.Hashtag;
        //         trendList.Add(new ShowTrendModel { Hashtag = hashtag, Count = count });
        //     }
        //     return trendList;
        // }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();

        }
    }
}