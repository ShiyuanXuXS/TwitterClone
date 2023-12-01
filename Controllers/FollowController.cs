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
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly TwitterCloneDbContext context;

        public FollowController(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, TwitterCloneDbContext context)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
        }

        [HttpGet("getFollowStatus")]
        public async Task<ActionResult<string>> getFollowStatusAsync(string userId)
        {
            Console.WriteLine("--------getFollowStatus--------");
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return StatusCode(400, "not authorized!");
            }
            var currentUserId = currentUser.Id;
            var followed = context.Follows.FirstOrDefault(f => f.User.Id == currentUserId && f.Author.Id == userId);

            if (followed != null)
            {
                // Record exists
                var result = new { followed = true, UserId = userId };
                return Ok(result);
            }
            else
            {
                // Record doesn't exist
                var result = new { followed = false, UserId = userId };
                return Ok(result);
            }
        }


        [HttpPost]
        [Route("HandleFollow")]
        public async Task<IActionResult> HandleFollow([FromBody] string userId)
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var userToFollow = await userManager.FindByIdAsync(userId);
            if (currentUser == null)
            {
                return StatusCode(400, "not authorized!");
            }
            if (userToFollow != null)
            {
                // Check if the follow relationship already exists
                var existingFollow = await context.Follows
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
                    context.Follows.Add(follow);
                    await context.SaveChangesAsync();
                    return Json(new { success = true, message = "Follow request sent successfully" });
                }
                else
                {
                    // If so, remove the follow relationship
                    context.Follows.Remove(existingFollow);
                    await context.SaveChangesAsync();
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