using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TwitterClone.Data;
using TwitterClone.Models;

namespace TwitterClone.Pages.UserPortal
{
    [Authorize]
    public class DeleteTweet : PageModel
    {
        private readonly ILogger<DeleteTweet> logger;
        private readonly TwitterCloneDbContext context;
        private readonly UserManager<User> userManager;

        public DeleteTweet(ILogger<DeleteTweet> logger, TwitterCloneDbContext context, UserManager<User> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
        }

        [BindProperty]
        public Tweet Tweet { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tweet = await context.Tweets.FirstOrDefaultAsync(t => t.Id == id);
            // _logger.LogInformation("-----------------------------------id:" +tweet.Id);
            var currentUser = await userManager.GetUserAsync(HttpContext.User);

            if (tweet == null || tweet.Author.Id != currentUser.Id)
            {
                return NotFound();
            }
            Tweet = tweet;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tweet = await context.Tweets.FindAsync(id);
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (tweet != null && tweet.Author.Id == currentUser.Id)
            {

                await context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Tweets SET ParentTweetId = NULL WHERE ParentTweetId = {tweet.Id}");
                // Tweet = tweet;
                context.Tweets.Remove(tweet);
                await context.SaveChangesAsync();
                TempData["Message"] = "Tweet deleted successfully!";
            }

            return RedirectToPage("./MyTweets");
        }
    }
}