using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.AspNetCore.Identity;
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
        public List<Tweet> SearchedTweet { get; set; } = null!;

        public SearchResultModel(ILogger<SearchResultModel> logger, TwitterCloneDbContext context, UserManager<User> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
        }

        public async Task OnGetAsync(string term)
        {
            SearchTerm = term;

            // get users where username, nickname, or description contains search term, case ignored
            Console.WriteLine("Search Term: " + SearchTerm);
            if (SearchTerm != null)
            {
                var searchedUser = context.Users
        .AsEnumerable() // Switch to client-side evaluation
        .Where(u =>
            (u.NickName != null && u.NickName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (u.Description != null && u.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (u.UserName != null && u.UserName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)))
        .ToList();
                Console.WriteLine("---------Searched User: " + searchedUser.Count);

                if (searchedUser.Count > 0)
                {
                    // get the current user
                    var currentUser = await userManager.GetUserAsync(HttpContext.User);
                    // exlcude current user from search result
                    searchedUser = searchedUser.Where(u => u.Id != currentUser.Id).ToList();
                    SearchedUser = searchedUser;
                    Console.WriteLine("Searched User: " + SearchedUser.Count);
                }
                else
                {
                    // No users found, handle this case
                    SearchedUser = new List<User>();
                    Console.WriteLine("No users found");
                }
            }
            else
            {
                Console.WriteLine("No users found");
            }

            // get tweets where content contains search term, case ignored
            if (SearchTerm != null)
            {
                var searchedTweet = context.Tweets.
                                AsEnumerable()
                                .Where(t => t.Body != null && t.Body.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                Console.WriteLine("---------Searched Tweet: " + searchedTweet.Count);
            }
            else
            {
                Console.WriteLine("No tweets found");
            }

        }
    }
}

