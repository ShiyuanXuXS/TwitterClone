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

namespace TwitterClone.Pages.ShowTweet
{
    [Authorize(Roles = "Admin")]
    public class SuspendTweet : PageModel
    {
        private readonly ILogger<SuspendTweet> logger;
        private readonly TwitterCloneDbContext context;
        private readonly UserManager<User> userManager;

        public SuspendTweet(ILogger<SuspendTweet> logger, TwitterCloneDbContext context, UserManager<User> userManager)
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

            if (tweet ==null)
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
            if (tweet != null )
            {
                tweet.Suspended=!tweet.Suspended;
                await context.SaveChangesAsync();
                TempData["Message"] = "Set tweet suspended status successfully!";
            }

            return RedirectToPage("/Home");
        }
    }
}