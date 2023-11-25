using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwitterClone.Data;
using TwitterClone.Models;

namespace TwitterClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController:ControllerBase
    {
        private readonly TwitterCloneDbContext _context;
        private readonly ILogger<LikeController> _logger;
        public LikeController(TwitterCloneDbContext context,ILogger<LikeController> logger){
            _context=context;
            _logger=logger;
        }
        [HttpGet("getLikeStatus")]
        public ActionResult<string> getLikeStatus(int tweetId)
        {
            var userId = "AAA111-TEST-COM"; // You need to replace this with the actual logged-in user's id
            var liked = _context.Likes.FirstOrDefault(like => like.User.Id == userId && like.Tweet.Id == tweetId);

            if (liked != null)
            {
                // Record exists
                var result = new { liked=true, TweetId = tweetId };
                return Ok(result);
            }
            else
            {
                // Record doesn't exist
                var result = new { liked=false,TweetId = tweetId };
                return Ok(result);
            }
        }
        [HttpPost("setLike")]
        public ActionResult<string> setLike(int tweetId)
        {
            var userId = "AAA111-TEST-COM"; // You need to replace this with the actual logged-in user's id
            var liked = _context.Likes.FirstOrDefault(like => like.User.Id == userId && like.Tweet.Id == tweetId);

            if (liked != null)
            {
                // Record exists
                try{
                    _context.Likes.Remove(liked);
                    _context.SaveChanges();
                    var result = new { liked=false, TweetId = tweetId };
                    return Ok(result);
                }catch(Exception ex){
                    _logger.LogError(ex.Message);
                    return StatusCode(500, "Internal server error.");
                }
                
            }
            else
            {
                // Record doesn't exist
                try{
                    Like newLike=new Like{
                        User=_context.Users.FirstOrDefault(u=>u.Id==userId),
                        Tweet=_context.Tweets.FirstOrDefault(t=>t.Id==tweetId),
                        CreatedAt=DateTime.Now
                    };
                    _context.Likes.Add(newLike);
                    _context.SaveChanges();
                    var result = new { liked=true,TweetId = tweetId };
                    return Ok(result);
                }catch(Exception ex){
                    _logger.LogError(ex.Message);
                    return StatusCode(500, "Internal server error.");
                }
                

                
            }
        }
    }
}