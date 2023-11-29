using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
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
    public class AddEditTweet : PageModel
    {
        private readonly ILogger<AddEditTweet> _logger;
        private readonly TwitterCloneDbContext _context;
        private readonly UserManager<User> _userManager;
        public AddEditTweet(ILogger<AddEditTweet> logger,TwitterCloneDbContext context,UserManager<User> userManager)
        {
            _logger = logger;
            _context=context;
            _userManager=userManager;
        }
        [BindProperty]
        public bool IsEditMode{get;set;}
        [BindProperty]
        public int TweetId{get;set;}
        [BindProperty]
        public int ReTweetId{get;set;}
        public Tweet? ReTweet{ get;set; }

        [BindProperty,Required,MinLength(10),MaxLength(20000)]
        public string Body{get;set;}

        public async Task<IActionResult> OnGetAsync(int? id,int? reTweetId)
        {
            // _logger.LogInformation("----------------------id:"+id);
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            
            IsEditMode=id.HasValue;
            if (IsEditMode){
                // Tweet tweet=await _context.Tweets.FirstOrDefaultAsync(t=>t.Id==id);
                Tweet tweet = await _context.Tweets
                    .Include(t => t.ParentTweet)
                    .ThenInclude(pt => pt.Author)
                    .FirstOrDefaultAsync(t => t.Id == id && t.Author.Id == currentUser.Id);
                //  _logger.LogInformation("---------------------"+tweet.ParentTweet.Author);
                // return Page();
                if (tweet==null){
                    return NotFound();
                }else{
                    Body=tweet.Body;
                    TweetId= (int)id;
                    if (tweet.ParentTweet!=null){
                        ReTweet=tweet.ParentTweet;
                        _logger.LogInformation("---------------------"+ReTweet.ParentTweet.Id);
                    }
                    
                }
            }else{
                if (reTweetId.HasValue){
                    // ReTweet=await _context.Tweets.Include(t => t.ParentTweet).FirstOrDefaultAsync(t=>t.Id==reTweetId);
                    // _logger.LogInformation("---------------------"+ReTweet.ParentTweet.Id);
                    ReTweet=GetFullReTweet((int)reTweetId);
                }
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(){
            // _logger.LogInformation(ReplaceTag(Body));
            // return Page();
            
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (!ModelState.IsValid ){
                return Page();
            }
            if (IsEditMode){
                // _logger.LogInformation("------------------------------Edit id="+TweetId);
                Tweet tweet=await _context.Tweets.FirstOrDefaultAsync(t=>t.Id==TweetId);
                if (tweet!=null){
                    if (tweet.Author.Id!=currentUser.Id){
                        return Page();
                    }
                    tweet.Body=ReplaceTag(Body);
                    _context.Attach(tweet).State = EntityState.Modified;
                }

            }else{
                _logger.LogInformation("------------------"+ReTweetId);
                if (ReTweetId>0){
                    ReTweet= await _context.Tweets.FirstOrDefaultAsync(t=>t.Id==ReTweetId);
                }
                Tweet tweet=new Tweet{
                    Body=ReplaceTag(Body),
                    ParentTweet=ReTweet,
                    CreatedAt=DateTime.Now,
                    //Todo set Author to logged user
                    Author=currentUser
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
    
        private Tweet? GetFullReTweet(int reTweetId)
        {
            Tweet? reTweet =  _context.Tweets
                .Include(t => t.ParentTweet)
                // .ThenInclude(pt => pt.Author)
                .FirstOrDefault(t => t.Id == reTweetId);

            if (reTweet!= null && reTweet.ParentTweet!=null)
            {
                reTweet.ParentTweet = GetFullReTweet(reTweet.ParentTweet.Id);
            }

            return reTweet;
        }

        static string ReplaceTag(string bodyString)
        {
            string pattern = @"#([a-zA-Z][a-zA-Z0-9_-]*)";
            Regex regex = new Regex(pattern);

            string result = regex.Replace(bodyString, match => $"#<a href='../SearchResult/?term={match.Groups[1].Value}'>{match.Groups[1].Value}</a>");

            return result;
        }
    }
}