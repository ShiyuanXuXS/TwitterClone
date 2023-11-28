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

        [BindProperty]
        public int indexPeople { get; set; } = 1;

        public int indexTweet { get; set; } = 1;


        public SearchResultModel(ILogger<SearchResultModel> logger, TwitterCloneDbContext context, UserManager<User> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
        }

        private List<User> PerformUserSearch(string searchTerm, User currentUser)
        {
            // Console.WriteLine("======Search Term: " + searchTerm);
            // Console.WriteLine("Current User: " + currentUser.Id);
            var res =
           context.Users
               .AsEnumerable() // Switch to client-side evaluation
               .Where(u =>
                   u.Id != currentUser.Id && // Exclude the current user
                   ((u.NickName != null && u.NickName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                   (u.Description != null && u.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                   (u.UserName != null && u.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))))
               .ToList();
            var followedUser = context.Follows.Where(f => f.User.Id == currentUser.Id).Select(f => f.Author).ToList();
            if (followedUser.Count > 0)
            {  // if current user is following someone, show users that current user is not following
                return res.Except(followedUser).ToList();
            }
            return res;
        }

        public async Task OnGetAsync(string term)
        {
            // get all users that current user followed
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            // get all users that current user is following
            SearchTerm = term;
            ViewData["SearchTerm"] = term;
            // get users where username, nickname, or description contains search term, case ignored
            if (SearchTerm != null)
            {
                // var currentUser = await userManager.GetUserAsync(HttpContext.User);
                var res = PerformUserSearch(SearchTerm, currentUser);
                SearchedUser = res.OrderBy(x => Guid.NewGuid()).Take(3).ToList();
                Console.WriteLine("---------Searched User: " + SearchedUser.Count);
            }
            else
            {
                Console.WriteLine("No users found");
                SearchedUser = new List<User>();
            }

            // get tweets where content contains search term, case ignored
            if (SearchTerm != null)
            {
                var searchedTweet = context.Tweets.
                                AsEnumerable()
                                .Where(t => t.Body != null && t.Body.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                Console.WriteLine("=======Searched Tweet=========: " + searchedTweet.Count);
            }
            else
            {
                Console.WriteLine("No tweets found");
            }
        }

        public async Task<IActionResult> OnPostShowAllPeople()
        {
            indexPeople = 0;
            indexTweet = 0;
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var searchTerm = Request.Form["searchTerm"].ToString();
            // Console.WriteLine("========in post Search Term: " + searchTerm);
            // Console.WriteLine("Current User: " + currentUser.Id);
            SearchedUser = PerformUserSearch(searchTerm, currentUser);
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
            SearchedUser = PerformUserSearch(searchTerm, currentUser).OrderBy(x => Guid.NewGuid()).Take(1).ToList();
            return Page();
        }
        public async Task<IActionResult> OnPostShowAllTweet()
        {
            indexPeople = 0;
            indexTweet = 1;
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var searchTerm = Request.Form["searchTerm"].ToString();
            // Console.WriteLine("========in post Search Term: " + searchTerm);
            // Console.WriteLine("Current User: " + currentUser.Id);
            // SearchedTweet = context.Tweets.
            //                     AsEnumerable()
            //                     .Where(t => t.Body != null && t.Body.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            return Page();
        }
    }
}

