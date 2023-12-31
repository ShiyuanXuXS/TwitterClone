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
        public int CurrentListOption { get; set; }

        public int FollowingCount { get; set; }
        public int FollowerCount { get; set; }

        public List<Follow> FollowList { get; set; }

        public UserModel(UserManager<User> userManager, ILogger<UserModel> logger, TwitterCloneDbContext db)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.db = db;
        }


        public async Task<IActionResult> OnGetAsync(int? pageNumber, int? listOption)
        {
            var user = db.Users.Where(u => u.UserName == Username).FirstOrDefault();
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            // if (user.Id!=currentUser.Id){
            //     Profile = user;
            //     Tweets=[];
            //     return Page();
            // }

            FollowerCount = db.Follows
            .Where(f => f.Author.Id == currentUser.Id)
            .Count();

            FollowingCount = db.Follows
            .Where(f => f.User.Id == currentUser.Id)
            .Count();


            var numberPerPage = 4;
            CurrentPage = (int)(pageNumber.HasValue ? pageNumber : 1);
            CurrentListOption = (int)(listOption.HasValue ? listOption : 0);


            IQueryable<Tweet> query = db.Tweets
                    .Where(t => t.Author.Id == user.Id)
                    .OrderByDescending(t => t.CreatedAt)
                    .Include(t => t.Author)
                    .Include(t => t.ParentTweet)
                        .ThenInclude(pt => pt.Author)
                    .Include(t => t.ParentTweet)
                        .ThenInclude(pt => pt.ParentTweet);


            if (listOption.HasValue && pageNumber.HasValue)
            {
                switch (listOption)
                {
                    case 0:
                        break;
                    case 1:
                        query = db.Tweets
                            .Where(t => db.Comments.Any(c => c.Tweet.Id == t.Id && c.Commenter.Id == user.Id))
                            .OrderByDescending(t => t.CreatedAt)
                            .Include(t => t.Author)
                            .Include(t => t.ParentTweet)
                                .ThenInclude(pt => pt.Author)
                            .Include(t => t.ParentTweet)
                                .ThenInclude(pt => pt.ParentTweet);
                        break;
                    case 2:
                        query = db.Tweets.Where(t => db.Likes.Any(l => l.Tweet.Id == t.Id && l.User.Id == user.Id))
                            .OrderByDescending(t => t.CreatedAt)
                            .Include(t => t.Author)
                            .Include(t => t.ParentTweet)
                                .ThenInclude(pt => pt.Author)
                            .Include(t => t.ParentTweet)
                                .ThenInclude(pt => pt.ParentTweet);
                        break;
                    case 3:
                        FollowList = await db.Follows
                            .Where(f => f.User.Id == currentUser.Id)
                            .OrderByDescending(t => t.CreatedAt)
                            .Include(f => f.Author)
                            .ToListAsync();
                        break;
                    case 4:
                        FollowList = await db.Follows
                            .Where(f => f.Author.Id == currentUser.Id)
                            .OrderByDescending(t => t.CreatedAt)
                            .Include(f => f.User)
                            .ToListAsync();
                        break;
                    default:
                        break;
                }

            }
            TotalPages = (int)Math.Ceiling(query.Count() / (double)numberPerPage);

            //if (listOption.HasValue && pageNumber.HasValue && listOption < 3) {
            Tweets = await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            Tweets = Tweets
                .Skip((pageNumber - 1) * numberPerPage ?? 0)
                .Take(numberPerPage)
                .ToList();
            //}

            //if (listOption.HasValue && pageNumber.HasValue && listOption > 2) {
            if (FollowList != null && FollowList.Any() == true) {

                FollowList = FollowList
                .Skip((pageNumber - 1) * numberPerPage ?? 0)
                .Take(numberPerPage)
                .ToList();
            }

            //}




            if (user != null)
            {
                // Profile = currentUser;
                Profile = user;
                return Page();

            }
            else
            {
                return RedirectToPage("NotFound");
            }

        }
    }
}
