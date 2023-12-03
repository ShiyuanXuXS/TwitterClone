using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;
using System.ComponentModel.DataAnnotations;


namespace TwitterClone.Pages
{
    public class ReportModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int? ReportTweetId { get; set; }

        private readonly ILogger<SearchResultModel> logger;
        private readonly TwitterCloneDbContext context;
        private readonly UserManager<User> userManager;

        [BindProperty]
        public Tweet ReportTweet { get; set; } = null!;
        public Message NewMessage { get; set; } = null!;
        public List<User> Admins { get; set; } = null!;

        [BindProperty]
        public ReportInputModel Input { get; set; } = null!;

        public class ReportInputModel
        {
            [Required]
            [Display(Name = "Detail Message")]
            public string Content { get; set; }

        }

        public ReportModel(ILogger<SearchResultModel> logger, TwitterCloneDbContext context, UserManager<User> userManager)
        {
            this.logger = logger;
            this.context = context;
            this.userManager = userManager;
            Input = new ReportInputModel();
        }

        public async Task OnGetAsync(int Id)
        {
            int? reportTweetId = Id;
            Console.WriteLine("get id: " + reportTweetId);
            if (reportTweetId == 0)
            {
                Console.WriteLine("ReportTweetId is null");
            }
            var tweet = await context.Tweets.Include(t => t.Author).FirstOrDefaultAsync(t => t.Id == reportTweetId);
            if (tweet == null)
            {
                Console.WriteLine("Tweet is null");
            }
            ReportTweet = tweet;
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is not valid");
                return Page();
            }
            var currentUser = await userManager.GetUserAsync(HttpContext.User);

            IList<User> admins = await userManager.GetUsersInRoleAsync("Admin");
            Console.WriteLine("admins: " + admins.Count);
            Console.WriteLine("here is good");
            if (Input.Content == null)
            {
                Console.WriteLine("Input.Content is null");
                return Page();
            }
            Console.WriteLine(Input.Content + " Report For Tweet: " + ReportTweet.Id);

            foreach (var admin in admins)
            {
                // Create a new message for each admin user
                var newMessage = new Message
                {
                    Sender = currentUser,
                    Receiver = admin,
                    Content = Input.Content + " Report For Tweet: " + ReportTweet.Id,
                    SentAt = DateTime.Now,
                    IsRead = false,
                };
                context.Messages.Add(newMessage);
            }
            await context.SaveChangesAsync();
            logger.LogInformation($"User {currentUser.UserName} has report a tweet {ReportTweet.Id}.");
            TempData["FlashMessage"] = "Thank you for your report. Please wait for our response.";
            return RedirectToPage("./Home");
        }
    }
}
