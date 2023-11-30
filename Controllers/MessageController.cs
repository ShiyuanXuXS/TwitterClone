using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.EntityFrameworkCore;

namespace TwitterClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly TwitterCloneDbContext db;

        public class Msg {
            public string Recipient { get; set; }
            public string Content { get; set; }
        }
        public MessageController(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, TwitterCloneDbContext db)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.db = db;
        }

        [HttpGet]
        public async Task<ActionResult<string>> getConversation(string userId)
        {
           var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var contact = await userManager.FindByIdAsync(userId);

            if (currentUser == null)
            {
                return StatusCode(401, "You must be logged in to send messages.");
            }
            if (contact == null)
            {
                return StatusCode(400, "User not found.");
            }
            
            var msgList = await db.Messages
            .Where(m =>(m.Receiver == contact & m.Sender == currentUser) || (m.Sender == contact & m.Receiver == currentUser))
            .Include(m => m.Receiver)
            .Include(m => m.Sender)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

            //FIXME exclude private user data
                return Json(new { msgList });
        }

        [HttpGet("Inbox")]
        public async Task<ActionResult<string>> getInbox()
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return StatusCode(401, "You must be logged in to send messages.");
            }
            var id = currentUser.Id;
            var inbox = db.Messages
            .Where(m => m.Receiver.Id == id)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

            //FIXME exclude private user data
                return Json(new { inbox });
        }

        [HttpGet("Outbox")]
        public async Task<ActionResult<string>> getOutbox()
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return StatusCode(401, "You must be logged in to send messages.");
            }
            var id = currentUser.Id;
            var outbox = db.Messages.Where(m => m.Sender.Id == id).OrderByDescending(m => m.SentAt).ToListAsync();

                return Ok(outbox);
        }

        //TODO validate msg body
        [HttpPost]
        public async Task<IActionResult> SendMsg([FromBody] Msg msg)
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var receiver = await userManager.FindByIdAsync(msg.Recipient);
            if (currentUser == null)
            {
                return StatusCode(401, "You must be logged in to send messages.");
            }
            if (receiver == null)
            {
                return StatusCode(400, "User not found.");
            }
            if (String.IsNullOrEmpty(msg.Content))
            {
                return StatusCode(400, "Message body must not be empty.");
            }

                    var newMsg = new Message
                    {
                        Sender = currentUser,
                        Receiver = receiver,
                        Content = msg.Content,
                        SentAt = DateTime.Now
                    };
                    db.Messages.Add(newMsg);
                    await db.SaveChangesAsync();
                    return Json(new { success = true, message = "Message sent successfully" });


        }
    }
}