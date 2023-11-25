using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TwitterClone.Data;
using TwitterClone.Models;

namespace TwitterClone.Pages.UserPortal
{
    public class DeleteTweet : PageModel
    {
        private readonly ILogger<DeleteTweet> _logger;
        private readonly TwitterCloneDbContext _context;

        public DeleteTweet(ILogger<DeleteTweet> logger,TwitterCloneDbContext context)
        {
            _logger = logger;
            _context=context;
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

            if (tweet == null)
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

            var tweet = await _context.Tweets.FindAsync(id);
            if (tweet != null)
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