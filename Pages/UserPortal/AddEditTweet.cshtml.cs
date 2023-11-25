using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class AddEditTweet : PageModel
    {
        private readonly ILogger<AddEditTweet> _logger;
        private readonly TwitterCloneDbContext _context;

        public AddEditTweet(ILogger<AddEditTweet> logger,TwitterCloneDbContext context)
        {
            _logger = logger;
            _context=context;
        }
        [BindProperty]
        public bool IsEditMode{get;set;}
        [BindProperty]
        public int TweetId{get;set;}

        [BindProperty,Required,MinLength(10),MaxLength(20000)]
        public string Body{get;set;}

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // _logger.LogInformation("----------------------id:"+id);
            
            IsEditMode=id.HasValue;
            if (IsEditMode){
                Tweet tweet=await _context.Tweets.FirstOrDefaultAsync(t=>t.Id==id);
                if (tweet==null){
                    return NotFound();
                }else{
                    Body=tweet.Body;
                    TweetId= (int)id;
                }
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(){
            
            if (!ModelState.IsValid){
                return Page();
            }
            if (IsEditMode){
                // _logger.LogInformation("------------------------------Edit id="+TweetId);
                Tweet tweet=await _context.Tweets.FirstOrDefaultAsync(t=>t.Id==TweetId);
                if (tweet!=null){
                    tweet.Body=Body;
                    _context.Attach(tweet).State = EntityState.Modified;
                }

            }else{
                Tweet tweet=new Tweet{
                    Body=Body,
                    CreatedAt=DateTime.Now,
                    //Todo set Author to logged user
                    Author=_context.Users.FirstOrDefault(u=>u.UserName=="AAA222")
                };
                _context.Tweets.Add(tweet);
                
                
            }
            
            try{
                await _context.SaveChangesAsync();
                TempData["Message"] = IsEditMode ? "Tweet updated successfully!" : "Tweet added successfully!";
                _logger.LogInformation("Tweet Saved");
            }catch(Exception e){
                ModelState.AddModelError(string.Empty,"Internal error posting the tweet");
                _logger.LogError(e.Message);
                return Page();
            };
            return RedirectToPage("MyTweets");
        }
    }
}