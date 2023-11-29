using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TwitterClone.Data;
using TwitterClone.Models;

namespace TwitterClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReplyController:ControllerBase
    {
        private readonly TwitterCloneDbContext _context;
        private readonly ILogger<LikeController> _logger;
        private readonly UserManager<User> _userManager;
        public ReplyController(TwitterCloneDbContext context,ILogger<LikeController> logger,UserManager<User> userManager){
            _context=context;
            _logger=logger;
            _userManager=userManager;
        }
        [HttpGet("getReplies")]
        public async Task<ActionResult<string>> getReplyAsync(int tweetId)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser==null){
                return StatusCode(400,"not authorized!");
            }
            var replies = _context.Comments.Where(c => c.Tweet.Id ==  tweetId).ToList();
            return Ok(replies);
        }
        [HttpPost("addReply")]
        public async Task<ActionResult<string>> postReplyAsync([FromBody] ReplyDto replyDto)
        {
            int tweetId = replyDto.TweetId;
            string replyBody = replyDto.ReplyBody;
            Comment reply;
            try{
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                reply=new Comment{
                    Commenter=currentUser,
                    Tweet=_context.Tweets.FirstOrDefault(t=>t.Id==tweetId),
                    CreatedAt=DateTime.Now,
                    Body=replyBody
                };
            }catch(Exception ex){
                _logger.LogError(ex.Message);
                return StatusCode(400,"Request error");
            };
            
            try{
                _context.Comments.Add(reply);
                _context.SaveChanges();
                return Ok(new{success=true});

            }catch(Exception ex){
                _logger.LogError(ex.Message);
                return StatusCode(500,"Internal server error.");
            }
            
        }
        public class ReplyDto
        {
            public int TweetId { get; set; }
            public string ReplyBody { get; set; }
        }
    }
}