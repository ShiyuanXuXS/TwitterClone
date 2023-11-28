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


namespace TwitterClone.Pages.Shared.Components.UserReplies
{
    public class UserRepliesViewComponent : ViewComponent
    {

        private readonly TwitterCloneDbContext db;

        public UserRepliesViewComponent(TwitterCloneDbContext db) {
            this.db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(string username)
        {
            var user = db.Users.Where(u => u.UserName == username).FirstOrDefault();

            List<Comment> replies = await db.Comments
                .Where(t=>t.Commenter.Id==user.Id)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(replies);
        }
    }
}
