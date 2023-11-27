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
        private readonly ILogger<DeleteTweet> _logger;
        private readonly TwitterCloneDbContext _context;
        private readonly UserManager<User> _userManager;

        public DeleteTweet(ILogger<DeleteTweet> logger,TwitterCloneDbContext context,UserManager<User> userManager)
        {
            _logger = logger;
            _context=context;
            _userManager=userManager;
        }

        [BindProperty]
        public Tweet Tweet{get;set;}= default!;
         public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tweet = await _context.Tweets.FirstOrDefaultAsync(t => t.Id == id);
            _logger.LogInformation("-----------------------------------id:" +tweet.Id);
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (tweet == null || tweet.Author.Id!=currentUser.Id)
            {
                return NotFound();
            }
                Tweet = tweet;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null )
            {
                return NotFound();
            }

            var tweet = await _context.Tweets.FindAsync(id);
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (tweet != null && tweet.Author.Id==currentUser.Id)
            {
                Tweet = tweet;
                _context.Tweets.Remove(tweet);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Todo deleted successfully!";
            }

            return RedirectToPage("./MyTweets");
        }
    }
}