using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TwitterClone.Data;
using TwitterClone.Models;

namespace TwitterClone.Pages.UserPortal
{
    public class AddTweet : PageModel
    {
        private readonly ILogger<AddTweet> _logger;
        private readonly TwitterCloneDbContext _context;

        public AddTweet(ILogger<AddTweet> logger,TwitterCloneDbContext context)
        {
            _logger = logger;
            _context=context;
        }
        [BindProperty]
        public string Title{get;set;}
        [BindProperty]
        public string Body{get;set;}

        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(){
            if (Body==null ||Body.Trim()=="") return Page();
            Tweet tweet=new Tweet{
                Title=Title==null? "":Title,
                Body=Body,
                CreatedAt=DateTime.Now
            };
            _context.Tweets.Add(tweet);
            await _context.SaveChangesAsync();

            return RedirectToPage("MyTweets");
        }
    }
}