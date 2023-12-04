using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;



namespace TwitterClone.Pages
{
    public class SearchResultModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        private readonly ILogger<SearchResultModel> logger;
        private readonly TwitterCloneDbContext context;
        private readonly UserManager<User> userManager;

        [BindProperty]
        public List<User> SearchedUser { get; set; } = null!;

        [BindProperty]
        public List<User> AllSearchedUser { get; set; } = null!;

        [BindProperty]
        public List<Tweet> SearchedTweet { get; set; } = null!;

        public List<User> FollowedUser { get; set; } = null!;

        public List<User> FollowSuggestion { get; set; } = null!;

        [BindProperty]
        public User? CurrentUser { get; set; }

        [BindProperty]
        public int indexPeople { get; set; } = 1;

        public int indexTweet { get; set; } = 1;

        public int ShowDescription { get; set; } = 1;  // 0: hide, 1: show
        public int indexView { get; set; } = 1;  // 1: top, 2: people, 3: tweet

        public int IndexDropDown { get; set; } = 1;  // 1: search/home page, 2: what'shapppening page

        public SearchResultModel(ILogger<SearchResultModel> logger, TwitterCloneDbContext context, UserManager<User> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
        }

        public List<ShowTrendModel>? ShowTrend { get; set; }

        private List<ShowTrendModel> getTrend(User currentUser)
        {
            var randomTweets = context.Tweets
        .Include(t => t.Author)
        .AsEnumerable()
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
        private void getUnFollowedUserList(User currentUser)
        {
            // get all users
            var allUsers = userManager.Users.ToList();
            Console.WriteLine("=======1. All Users: " + allUsers.Count);
            // get all users that current user is following
            var followedUser = context.Follows.Where(f => f.User.Id == currentUser.Id).Select(f => f.Author).ToList();
            Console.WriteLine("=======2. Followed User: " + followedUser.Count);
            if (followedUser.Count > 0)
            {  // if current user is following someone, show users that current user is not following
                FollowedUser = followedUser;
                var excludeFollowed = allUsers.Except(FollowedUser).ToList();
                FollowSuggestion = excludeFollowed.Where(user => user.Id != currentUser.Id).ToList();
                Console.WriteLine("==========3.Follow Suggestion: " + FollowSuggestion.Count);
            }
            else
            {  // if current user is not following anyone, show all users
                FollowedUser = new List<User>();
                FollowSuggestion = allUsers.Where(user => user.Id != currentUser.Id).ToList();
                Console.WriteLine("==========4.Follow Suggestion: " + FollowSuggestion.Count);
            }
            // return FollowSuggestion;
        }



        private List<User> PerformUserSearch(string searchTerm, User currentUser)
        {
            //query users that username/nickname/description contain search term
            var res =
           context.Users
               .AsEnumerable() // Switch to client-side evaluation
               .Where(u =>
                   u.Id != currentUser.Id && // Exclude the current user
                   ((u.NickName != null && u.NickName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                   (u.Description != null && u.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                   (u.UserName != null && u.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))))
               .ToList();
            return res;
        }

        private List<Tweet> PerformFollowedTweetSearch(string searchTerm, User currentUser)
        {
            getUnFollowedUserList(currentUser);

            //query tweets that followed user username/nickname or body contain search term
            var res =
           context.Tweets
            .Include(t => t.Author)
               .AsEnumerable() // Switch to client-side evaluation
               .Where(t =>
                    t.Author != null &&
                    t.Author.Id != currentUser.Id &&
                    FollowedUser.Contains(t.Author) &&
                   t.Body != null && t.Body.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                   || t.Author.NickName != null && t.Author.NickName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                   || t.Author.UserName != null && t.Author.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
               .ToList();
            return res;
        }

        private List<Tweet> PerformTweetSearch(string searchTerm, User currentUser)
        {

            //query tweets that author username/nickname or body contain search term
            var res =
           context.Tweets
           .Include(t => t.Author)
               .AsEnumerable() // Switch to client-side evaluation
               .Where(t =>
                    t.Author != null && t.Author.Id != currentUser.Id && // Exclude the current user
                   t.Body != null && t.Body.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                   || t.Author.NickName != null && t.Author.NickName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                   || t.Author.UserName != null && t.Author.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
               .ToList();
            return res;
        }


        public async Task OnGetAsync(string term)
        {
            ViewData["SearchOptionValue"] = null;

            Console.WriteLine("==========search result enter");
            // get all users that current user followed
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            // get all users that current user is following
            SearchTerm = term;
            ViewData["SearchTerm"] = term;
            if (SearchTerm != null)
            {
                // search users and tweets
                var resUser = PerformUserSearch(SearchTerm, currentUser);
                SearchedUser = resUser.OrderBy(x => Guid.NewGuid()).Take(3).ToList();
                var resTweet = PerformTweetSearch(SearchTerm, currentUser);
                SearchedTweet = resTweet.OrderBy(x => Guid.NewGuid()).Take(5).ToList();
                getUnFollowedUserList(currentUser);
                ShowTrend = getTrend(currentUser);
            }
            else
            {
                SearchedUser = new List<User>();
                SearchedTweet = new List<Tweet>();
            }
        }

        public async Task<IActionResult> OnPostShowAllPeople(string searchOptionValue)
        {
            ViewData["SearchOptionValue"] = searchOptionValue;

            Console.WriteLine("==========show all people enter");
            indexPeople = 0;
            indexTweet = 0;
            indexView = 2;
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            getUnFollowedUserList(currentUser);
            ShowTrend = getTrend(currentUser);
            var searchTerm = Request.Form["searchTerm"].ToString();
            if (searchOptionValue == "anyone" || searchOptionValue == null) { SearchedUser = PerformUserSearch(searchTerm, currentUser); }
            else
            {
                SearchedUser = PerformUserSearch(searchTerm, currentUser).Except(FollowSuggestion).ToList();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostShowTweetAndUser(string searchOptionValue)
        {
            ViewData["SearchOptionValue"] = searchOptionValue;
            Console.WriteLine("==========show tweet and user enter");
            indexPeople = 1;
            indexTweet = 1;
            indexView = 1;
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var searchTerm = Request.Form["searchTerm"].ToString();
            getUnFollowedUserList(currentUser);
            ShowTrend = getTrend(currentUser);
            if (searchOptionValue == "anyone" || searchOptionValue == null)
            {
                SearchedUser = PerformUserSearch(searchTerm, currentUser);
                SearchedTweet = PerformTweetSearch(searchTerm, currentUser);
            }
            else
            {
                if (FollowedUser.Count == 0)
                {
                    SearchedUser = new List<User>();
                    SearchedTweet = new List<Tweet>();
                }
                else
                {
                    SearchedUser = PerformUserSearch(searchTerm, currentUser).Except(FollowSuggestion).ToList();
                    var searchedTweet = PerformFollowedTweetSearch(searchTerm, currentUser).ToList();
                    if (searchedTweet.Count == 0)
                    {
                        SearchedTweet = new List<Tweet>();
                    }
                    else
                    {
                        SearchedTweet = searchedTweet;
                    }
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPostShowAllTweet(string searchOptionValue)
        {
            ViewData["SearchOptionValue"] = searchOptionValue;

            indexPeople = 0;
            indexTweet = 1;
            indexView = 3;
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var searchTerm = Request.Form["searchTerm"].ToString();
            getUnFollowedUserList(currentUser);
            ShowTrend = getTrend(currentUser);
            if (searchOptionValue == "anyone" || searchOptionValue == null)
            {
                SearchedUser = PerformUserSearch(searchTerm, currentUser);
                SearchedTweet = PerformTweetSearch(searchTerm, currentUser);
            }
            else
            {
                if (FollowedUser.Count == 0)
                {
                    SearchedUser = new List<User>();
                    SearchedTweet = new List<Tweet>();
                }
                else
                {
                    SearchedUser = PerformUserSearch(searchTerm, currentUser).Except(FollowSuggestion).ToList();
                    var searchedTweet = PerformFollowedTweetSearch(searchTerm, currentUser).ToList();
                    if (searchedTweet.Count == 0)
                    {
                        SearchedTweet = new List<Tweet>();
                    }
                    else
                    {
                        SearchedTweet = searchedTweet;
                    }
                }
            }
            return Page();
        }
    }
    public class ShowTrendModel
    {
        public int Id { get; set; }
        public List<string>? Hashtag { get; set; }
        public int CountLikes { get; set; }
        public int CountRetweets { get; set; }
    }

}

