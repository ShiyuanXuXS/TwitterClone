using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TwitterClone.Models;
using Microsoft.AspNetCore.Identity;
using TwitterClone.Models;
using TwitterClone.Data;
using Microsoft.EntityFrameworkCore;


namespace TwitterClone.Pages.Shared.Components.UserTweets
{
    public class UserTweetsViewComponent : ViewComponent
    {

        private readonly TwitterCloneDbContext db;

        public UserTweetsViewComponent(TwitterCloneDbContext db) {
            this.db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string username)
        {
            var user = db.Users.Where(u => u.UserName == username).FirstOrDefault();

            List<Tweet> tweets = await db.Tweets
                .Where(t=>t.Author.Id==user.Id)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tweets);
        }
    }
}
