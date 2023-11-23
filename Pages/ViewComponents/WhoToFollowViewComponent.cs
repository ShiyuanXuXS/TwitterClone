using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


namespace TwitterClone.Pages.ViewComponents
{
    public class WhoToFollowViewComponent : ViewComponent
    {

        private readonly UserManager<User> _userManager;
        private readonly ILogger<MainModel> _logger;
        private readonly TwitterCloneDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public new List<User> User { get; set; } = null!;
        public List<User> FollowdeUser { get; set; } = null!;

        [BindProperty]
        public User? CurrentUser { get; set; }



        public WhoToFollowViewComponent(UserManager<User> userManager, ILogger<MainModel> logger, TwitterCloneDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // get all users
            User = await _userManager.Users.ToListAsync();
            // get current user
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (currentUser != null)
            {
                CurrentUser = currentUser;
                // get all users that current user is following
                // var followdeUser = await _context.Follow.Where(f => f.Author.Id == currentUser.Id).Select(f => f.User).ToListAsync();
            }

            return View(User);
        }



    }
}