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
        private readonly ILogger<AddEditTweet> logger;
        private readonly TwitterCloneDbContext context;
        private readonly UserManager<User> userManager;
        public AddEditTweet(ILogger<AddEditTweet> logger, TwitterCloneDbContext context, UserManager<User> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
        }
        [BindProperty]
        public bool IsEditMode { get; set; }
        [BindProperty]
        public int TweetId { get; set; }
        [BindProperty]
        public int ReTweetId { get; set; }
        public Tweet? ReTweet { get; set; }

        [BindProperty, Required, MinLength(10), MaxLength(20000)]
        public string Body { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? reTweetId)
        {
            // _logger.LogInformation("----------------------id:"+id);
            var currentUser = await userManager.GetUserAsync(HttpContext.User);

            IsEditMode = id.HasValue;
            if (IsEditMode)
            {
                // Tweet tweet=await context.Tweets.FirstOrDefaultAsync(t=>t.Id==id);
                // Tweet tweet = await context.Tweets
                //     .Include(t=>t.Author)
                //     .Include(t => t.ParentTweet)
                //         .ThenInclude(pt => pt.ParentTweet)
                //     .FirstOrDefaultAsync(t => t.Id == id && t.Author.Id == currentUser.Id);
                //  _logger.LogInformation("---------------------"+tweet.ParentTweet.Author);
                // return Page();
                Tweet tweet=GetFullTweet((int)id);
                if (tweet == null)
                {
                    return NotFound();
                }
                else
                {
                    Body = tweet.Body;
                    TweetId = (int)id;
                    if (tweet.ParentTweet != null)
                    {
                        ReTweet = tweet.ParentTweet;
                        // logger.LogInformation("---------------------" + ReTweet.ParentTweet.Id);
                    }

                }
            }
            else
            {
                if (reTweetId.HasValue)
                {
                    // ReTweet=await context.Tweets.Include(t => t.ParentTweet).FirstOrDefaultAsync(t=>t.Id==reTweetId);
                    // _logger.LogInformation("---------------------"+ReTweet.ParentTweet.Id);
                    ReTweet = GetFullTweet((int)reTweetId);
                    // ReTweet=context.Tweets
                    //     .Include(t => t.Author)
                    //     .Include(t => t.ParentTweet)
                    //         .ThenInclude(pt => pt.Author)
                    //     .Include(t => t.ParentTweet)
                    //         .ThenInclude(pt => pt.ParentTweet)                        
                    //     .FirstOrDefault(t => t.Id == reTweetId);

                }
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            // _logger.LogInformation(ReplaceTag(Body));
            // return Page();

            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (IsEditMode)
            {
                // _logger.LogInformation("------------------------------Edit id="+TweetId);
                Tweet tweet = await context.Tweets.FirstOrDefaultAsync(t => t.Id == TweetId);
                if (tweet != null)
                {
                    if (tweet.Author.Id != currentUser.Id)
                    {
                        return Page();
                    }
                    tweet.Body = ReplaceTag(Body);
                    context.Attach(tweet).State = EntityState.Modified;
                }

            }
            else
            {
                logger.LogInformation("------------------" + ReTweetId);
                if (ReTweetId > 0)
                {
                    // ReTweet = await context.Tweets.FirstOrDefaultAsync(t => t.Id == ReTweetId);
                    // ReTweet=context.Tweets
                    //     .Include(t => t.Author)
                    //     .Include(t => t.ParentTweet)
                    //         .ThenInclude(pt => pt.Author)
                    //     .Include(t => t.ParentTweet)
                    //         .ThenInclude(pt => pt.ParentTweet)          
                    //     .FirstOrDefault(t => t.Id == ReTweetId);
                    ReTweet=GetFullTweet(ReTweetId);
                }
                Tweet tweet = new Tweet
                {
                    Body = ReplaceTag(Body),
                    ParentTweet = ReTweet,
                    CreatedAt = DateTime.Now,
                    Author = currentUser
                };
                context.Tweets.Add(tweet);
            }

            try
            {
                await context.SaveChangesAsync();
                TempData["Message"] = IsEditMode ? "Tweet updated successfully!" : "Tweet added successfully!";
                logger.LogInformation("Tweet Saved");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, "Internal error posting the tweet");
                logger.LogError(e.Message);
                return Page();
            };
            return RedirectToPage("/User", new { username = currentUser.UserName });

        }

        private Tweet? GetFullTweet(int reTweetId)
        {
            Tweet? reTweet = context.Tweets
                .Include(pt => pt.Author)
                .Include(t => t.ParentTweet)
                
                .FirstOrDefault(t => t.Id == reTweetId);

            if (reTweet != null && reTweet.ParentTweet != null)
            {
                reTweet.ParentTweet = GetFullTweet(reTweet.ParentTweet.Id);
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