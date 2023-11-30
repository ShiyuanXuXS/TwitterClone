using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TwitterClone.Data;
using TwitterClone.Models;

namespace TwitterClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly TwitterCloneDbContext context;
        private readonly ILogger<LikeController> logger;
        private readonly UserManager<User> userManager;
        public LikeController(TwitterCloneDbContext context, ILogger<LikeController> logger, UserManager<User> userManager)
        {
            this.context = context;
            this.logger = logger;
            this.userManager = userManager;
        }

        [HttpGet("getLikeStatus")]
        public async Task<ActionResult<string>> getLikeStatusAsync(int tweetId)
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return StatusCode(400, "not authorized!");
            }
            var userId = currentUser.Id;
            var liked = context.Likes.FirstOrDefault(like => like.User.Id == userId && like.Tweet.Id == tweetId);

            if (liked != null)
            {
                // Record exists
                var result = new { liked = true, TweetId = tweetId };
                return Ok(result);
            }
            else
            {
                // Record doesn't exist
                var result = new { liked = false, TweetId = tweetId };
                return Ok(result);
            }
        }
        [HttpPost("setLike")]
        public async Task<ActionResult<string>> setLikeAsync(int tweetId)
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return StatusCode(400, "not authorized!");
            }
            var userId = currentUser.Id;
            var liked = context.Likes.FirstOrDefault(like => like.User.Id == userId && like.Tweet.Id == tweetId);

            if (liked != null)
            {
                // _logger.LogInformation("+++++++++++++++++++++++++++++likeId:"+liked.Id.ToString());
                // Record exists
                try
                {
                    context.Likes.RemoveRange(liked);
                    context.SaveChanges();
                    var result = new { liked = false, TweetId = tweetId };
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    return StatusCode(500, "Internal server error.");
                }

            }
            else
            {
                // Record doesn't exist
                // _logger.LogInformation("+++++++++++++++++++++++++++++ new Like: UserId:"+currentUser.Id+" TweetId: "+tweetId.ToString());
                try
                {
                    Like newLike = new Like
                    {
                        User = context.Users.FirstOrDefault(u => u.Id == currentUser.Id),
                        Tweet = context.Tweets.FirstOrDefault(t => t.Id == tweetId),
                        CreatedAt = DateTime.Now
                    };
                    context.Likes.Add(newLike);
                    context.SaveChanges();
                    var result = new { liked = true, TweetId = tweetId };
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    return StatusCode(500, "Internal server error.");
                }



            }
        }
    }
}