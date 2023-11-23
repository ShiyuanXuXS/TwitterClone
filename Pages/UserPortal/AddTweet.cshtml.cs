using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [BindProperty,Required,MinLength(10),MaxLength(20000)]
        public string Body{get;set;}

        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(){
            // if (Body==null ||Body.Trim()=="") return Page();
            if (!ModelState.IsValid){
                return Page();
            }
            Tweet tweet=new Tweet{
                Body=Body,
                CreatedAt=DateTime.Now,
                // Todo: add Author here
            
            };
            try{
                _context.Tweets.Add(tweet);
                await _context.SaveChangesAsync();
            }catch(Exception e){
                ModelState.AddModelError(string.Empty,"Internal error posting the tweet");
                _logger.LogError(e.Message);
                return Page();
            }
           

            return RedirectToPage("MyTweets");
        }
    }
}