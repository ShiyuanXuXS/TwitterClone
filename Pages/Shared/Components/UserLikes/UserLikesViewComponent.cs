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


namespace TwitterClone.Pages.Shared.Components.UserLikes
{
    public class UserLikesViewComponent : ViewComponent
    {

        private readonly TwitterCloneDbContext db;

        public UserLikesViewComponent(TwitterCloneDbContext db) {
            this.db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string username)
        {
            var user = db.Users.Where(u => u.UserName == username).FirstOrDefault();

            List<Like> likes = await db.Likes
                .Where(l => l.User.Id == user.Id)
                .Include(l => l.Tweet)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
                


            List<Tweet> tweets = new List<Tweet>();

            if (likes != null) {
            foreach (Like like in likes) {

                //FIXME NULL ERRORS
                var likedTweet = await db.Tweets.Where(t => t.Id == like.Tweet.Id)                
                .Include(t => t.Author)
                .Include(t => t.ParentTweet)
                .FirstOrDefaultAsync();

                if (likedTweet != null) {
                tweets.Add(likedTweet);

                }
            }
            }



            return View(tweets);

        }
    }
}
