using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using TwitterClone.Models;
using TwitterClone.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace TwitterClone.Pages
{
    [Authorize]
    public class UserModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly ILogger<UserModel> logger;
        private readonly TwitterCloneDbContext db;

        [BindProperty(SupportsGet = true)]
        public string Username { get; set; }

        public User? Profile { get; set; }
        public IList<Tweet> Tweets { get; set; } = default!;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int CurrentListOption{get;set;}

        public UserModel(UserManager<User> userManager, ILogger<UserModel> logger, TwitterCloneDbContext db)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.db = db;
        }


        public async Task<IActionResult> OnGetAsync(int? pageNumber,int? listOption)
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var numberPerPage=3;
            CurrentPage= (int)(pageNumber.HasValue?pageNumber:1);
            CurrentListOption=(int)(listOption.HasValue?listOption:0);
            IQueryable<Tweet> query = db.Tweets
                    .Where(t => t.Author.Id == currentUser.Id)
                    .Include(t => t.ParentTweet)
                        .ThenInclude(pt => pt.ParentTweet);
            if (listOption.HasValue && pageNumber.HasValue){
                switch (listOption)
                {
                    case 0:
                        break;
                    case 1:
                        query = db.Tweets
                            .Where(t => db.Comments.Any(c => c.Tweet.Id == t.Id && c.Commenter.Id == currentUser.Id))
                            .Include(t => t.ParentTweet)
                                .ThenInclude(pt => pt.ParentTweet);
                        break;
                    case 2:
                        query = db.Tweets.Where(t => db.Likes.Any(l => l.Tweet.Id == t.Id && l.User.Id == currentUser.Id))
                            .Include(t => t.ParentTweet)
                                .ThenInclude(pt => pt.ParentTweet);
                        break;
                    default:
                        break;
                }
                    
            }
            TotalPages = (int)Math.Ceiling(query.Count() / (double)numberPerPage);
            Tweets = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * numberPerPage ?? 0)
                .Take(numberPerPage)
                .ToListAsync();

            if (currentUser != null)
            {
                Profile = currentUser;
                return Page();

            }
            else
            {
                return RedirectToPage("NotFound");
            }
            
        }
    }
}
