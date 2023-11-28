using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.EntityFrameworkCore;

namespace TwitterClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TwitterCloneDbContext _context;

        public FollowController(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, TwitterCloneDbContext context)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }


        [HttpPost]
        [Route("HandleFollow")]
        public async Task<IActionResult> HandleFollow([FromBody] string userId)
        {
            Console.WriteLine("--------HandleFollow--------");
            // Get the current user
            // var currentUser = await _userManager.FindByIdAsync("e1ea2e3b-d50f-4cc9-adaa-050b57eb412d");  // test user
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var userToFollow = await _userManager.FindByIdAsync(userId);

            if (currentUser != null && userToFollow != null)
            {
                // Check if the follow relationship already exists
                var existingFollow = await _context.Follows
                    .FirstOrDefaultAsync(f => f.User.Id == currentUser.Id && f.Author.Id == userToFollow.Id);

                if (existingFollow == null)
                {
                    // If not, add a new follow relationship
                    var follow = new Follow
                    {
                        User = currentUser,
                        Author = userToFollow,
                        CreatedAt = DateTime.Now
                    };
                    _context.Follows.Add(follow);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Follow request sent successfully" });
                }
                else
                {
                    // If so, remove the follow relationship
                    _context.Follows.Remove(existingFollow);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "UnFollow request sent successfully" });
                }
            }
            else
            {
                return Json(new { success = false, message = "User or User to follow not found" });
            }
        }
    }
}