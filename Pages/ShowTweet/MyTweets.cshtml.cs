using System;
using System.Collections.Generic;
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
    public class MyTweets : PageModel
    {
        private readonly ILogger<MyTweets> _logger;
    private readonly TwitterCloneDbContext _context;
        public MyTweets(ILogger<MyTweets> logger,TwitterCloneDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IList<Tweet> Tweets { get;set; } = default!;

        public async Task OnGetAsync()
        {
            User user=_context.Users.Where(u=>u.UserName=="AAA222").FirstOrDefault();
            //Todo get logged user
            Tweets = await _context.Tweets.Where(t=>t.Author.UserName==user.UserName).ToListAsync();
        }
    }
}