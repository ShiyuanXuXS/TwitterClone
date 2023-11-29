using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using System.Text.Json;


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

        public SearchResultModel(ILogger<SearchResultModel> logger, TwitterCloneDbContext context, UserManager<User> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
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
            // var followedUser = context.Follows.Where(f => f.User.Id == currentUser.Id).Select(f => f.Author).ToList();
            // if (followedUser.Count > 0)
            // {  // if current user is following someone, show users that current user is not following
            //     return res.Except(followedUser).ToList();
            // }
            return res;
        }

        private List<Tweet> PerformTweetSearch(string searchTerm, User currentUser)
        {
            //query tweets that author username/nickname or body contain search term
            var res =
           context.Tweets
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
                Console.WriteLine("---------Searched User: " + SearchedUser.Count);
                var resTweet = PerformTweetSearch(SearchTerm, currentUser);
                SearchedTweet = resTweet.OrderBy(x => Guid.NewGuid()).Take(5).ToList();
                Console.WriteLine("=======Searched Tweet=========: " + SearchedTweet.Count);
                getUnFollowedUserList(currentUser);
            }
            else
            {
                Console.WriteLine("No users found");
                SearchedUser = new List<User>();
                SearchedTweet = new List<Tweet>();
            }
        }

        public async Task<IActionResult> OnPostShowAllPeople(string searchOptionValue)
        {
            indexPeople = 0;
            indexTweet = 0;
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            getUnFollowedUserList(currentUser);
            var searchTerm = Request.Form["searchTerm"].ToString();
            if (searchOptionValue == "anyone") { SearchedUser = PerformUserSearch(searchTerm, currentUser); }
            else
            {
                SearchedUser = PerformUserSearch(searchTerm, currentUser).Except(FollowSuggestion).ToList();
            }


            return Page();
        }

        public async Task<IActionResult> OnPostShowTweetAndUser()
        {
            indexPeople = 1;
            indexTweet = 1;
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var searchTerm = Request.Form["searchTerm"].ToString();
            // Console.WriteLine("========in post Search Term: " + searchTerm);
            // Console.WriteLine("Current User: " + currentUser.Id);
            SearchedUser = PerformUserSearch(searchTerm, currentUser).OrderBy(x => Guid.NewGuid()).Take(3).ToList();
            SearchedTweet = PerformTweetSearch(searchTerm, currentUser).OrderBy(x => Guid.NewGuid()).Take(5).ToList(); ;
            getUnFollowedUserList(currentUser);
            return Page();
        }
        public async Task<IActionResult> OnPostShowAllTweet()
        {

            indexPeople = 0;
            indexTweet = 1;
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var searchTerm = Request.Form["searchTerm"].ToString();
            SearchedUser = PerformUserSearch(searchTerm, currentUser);
            SearchedTweet = PerformTweetSearch(searchTerm, currentUser);
            getUnFollowedUserList(currentUser);
            return Page();
        }
    }
}

