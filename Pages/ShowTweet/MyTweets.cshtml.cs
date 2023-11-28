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
        private readonly ILogger<MyTweets> _logger;
        private readonly TwitterCloneDbContext _context;
        private readonly UserManager<User> _userManager;
        public MyTweets(ILogger<MyTweets> logger,TwitterCloneDbContext context,UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager=userManager;
        }
        public IList<Tweet> Tweets { get;set; } = default!;

        public async Task OnGetAsync()
        {
            // User user=_context.Users.Where(u=>u.UserName=="AAA222").FirstOrDefault();
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            //Todo get logged user
            Tweets = await _context.Tweets
                .Where(t=>t.Author.Id==currentUser.Id)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}