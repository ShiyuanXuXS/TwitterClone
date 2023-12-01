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
    public class MyTweets : PageModel
    {
        private readonly ILogger<MyTweets> logger;
        private readonly TwitterCloneDbContext context;
        private readonly UserManager<User> userManager;
        public MyTweets(ILogger<MyTweets> logger, TwitterCloneDbContext context, UserManager<User> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
        }
        public IList<Tweet> Tweets { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // User user=context.Users.Where(u=>u.UserName=="AAA222").FirstOrDefault();
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            //Todo get logged user
            Tweets = await context.Tweets
                .Where(t => t.Author.Id == currentUser.Id)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}