using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;
        public LikeController(TwitterCloneDbContext context,ILogger<LikeController> logger,UserManager<User> userManager){
            _context=context;
            _logger=logger;
            _userManager=userManager;
        }
        [HttpGet("getLikeStatus")]
        public async Task<ActionResult<string>> getLikeStatusAsync(int tweetId)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser==null){
                return StatusCode(400,"not authorized!");
            }
            var userId=currentUser.Id;
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
        public async Task<ActionResult<string>> setLikeAsync(int tweetId)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser==null){
                return StatusCode(400,"not authorized!");
            }
            var userId=currentUser.Id;
            var liked = _context.Likes.FirstOrDefault(like => like.User.Id == userId && like.Tweet.Id == tweetId);

            if (liked != null)
            {
                _logger.LogInformation("+++++++++++++++++++++++++++++likeId:"+liked.Id.ToString());
                // Record exists
                try{
                    _context.Likes.RemoveRange(liked);
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
                _logger.LogInformation("+++++++++++++++++++++++++++++ new Like: UserId:"+currentUser.Id+" TweetId: "+tweetId.ToString());
                try{
                    Like newLike=new Like{
                        User=_context.Users.FirstOrDefault(u=>u.Id==currentUser.Id),
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