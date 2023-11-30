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
    public class ReplyController : ControllerBase
    {
        private readonly TwitterCloneDbContext context;
        private readonly ILogger<LikeController> logger;
        private readonly UserManager<User> userManager;
        public ReplyController(TwitterCloneDbContext context, ILogger<LikeController> logger, UserManager<User> userManager)
        {
            this.context = context;
            this.logger = logger;
            this.userManager = userManager;
        }
        [HttpGet("getReplies")]
        public async Task<ActionResult<string>> getReplyAsync(int tweetId)
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return StatusCode(400, "not authorized!");
            }
            var replies = context.Comments
                .Where(c => c.Tweet.Id == tweetId)
                .Select(c => new
                {
                    Commenter = new
                    {
                        Avatar = c.Commenter.Avatar,
                        NickName = c.Commenter.NickName,
                        UserName = c.Commenter.UserName
                    },
                    Body = c.Body,
                    CreatedAt = c.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToList();
            return Ok(replies);
        }
        [HttpPost("addReply")]
        public async Task<ActionResult<string>> postReplyAsync([FromBody] ReplyDto replyDto)
        {
            int tweetId = replyDto.TweetId;
            string replyBody = replyDto.ReplyBody;
            Comment reply;
            try
            {
                var currentUser = await userManager.GetUserAsync(HttpContext.User);
                reply = new Comment
                {
                    Commenter = currentUser,
                    Tweet = context.Tweets.FirstOrDefault(t => t.Id == tweetId),
                    CreatedAt = DateTime.Now,
                    Body = replyBody
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(400, "Request error");
            };

            try
            {
                context.Comments.Add(reply);
                context.SaveChanges();
                var responseData = new
                {
                    Success = true,
                    ReplyData = new
                    {
                        Commenter = new
                        {
                            Avatar = reply.Commenter.Avatar,
                            NickName = reply.Commenter.NickName,
                            UserName = reply.Commenter.UserName
                        },
                        CreatedAt = reply.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"), // 格式化 CreatedAt 字段
                        Body = reply.Body
                    }
                };

                return Ok(responseData);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error.");
            }

        }
        public class ReplyDto
        {
            public int TweetId { get; set; }
            public string ReplyBody { get; set; }
        }
    }
}