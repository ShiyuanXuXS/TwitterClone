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

        public class Msg
        {
            public string Recipient { get; set; }
            public string Content { get; set; }
        }

        public class Conversation
        {
            public User Contact { get; set; }
            public Message Msg { get; set; }
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
            .Where(m => (m.Receiver == contact & m.Sender == currentUser) || (m.Sender == contact & m.Receiver == currentUser))
            .Include(m => m.Receiver)
            .Include(m => m.Sender)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

            foreach (Message m in msgList) {
                m.SentAt = m.SentAt.ToUniversalTime();
                Console.WriteLine("-------------------------------- ");
                Console.WriteLine(m.SentAt);
                Console.WriteLine("-------------------------------- ");

            }
            //FIXME exclude private user data
            return Json(new { msgList });
        }

        [HttpGet("New")]
        public async Task<ActionResult<string>> getUser(string username)
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var contact = await userManager.FindByNameAsync(username);

            if (currentUser == null)
            {
                return StatusCode(401, "You must be logged in to send messages.");
            }
            if (contact == null)
            {
                return StatusCode(400, "User not found.");
            }


            //FIXME exclude private user data
            return Json(new { contact });
        }

        [HttpGet("Check")]
        public async Task<ActionResult<string>> checkMsgs()
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return StatusCode(401, "You must be logged in to check messages.");
            }
            var id = currentUser.Id;

            int unread = db.Messages
            .Where(m => m.Receiver.Id == id && m.IsRead == false)
            .Count();

            return Json(unread);
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

            var sent = await db.Messages
            .Where(m => m.Sender.Id == id)
            .Include(m => m.Receiver)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

            var received = await db.Messages
            .Where(m => m.Receiver.Id == id)
            .OrderByDescending(m => m.SentAt)
            .Include(m => m.Sender)
            .ToListAsync();

            var conversations = new List<Conversation>();
            var contacts = new List<User>();
            foreach (Message msg in sent)
            {
                if (!contacts.Contains(msg.Receiver))
                {
                    contacts.Add(msg.Receiver);
                    Console.WriteLine("--------------");
                    Console.WriteLine(msg.Receiver.UserName);

                    Console.WriteLine("--------------");
                }
            }
            foreach (Message msg in received)
            {
                if (!contacts.Contains(msg.Sender))
                {
                    contacts.Add(msg.Sender);
                    Console.WriteLine("--------------");
                    Console.WriteLine(msg.Sender.UserName);

                    Console.WriteLine("--------------");

                }
            }

            if (contacts.Any() == true)
            {
                foreach (User contact in contacts)
                {
                    Conversation convo = new Conversation();
                    convo.Contact = contact;
                    convo.Msg = await db.Messages
                .Where(m => (m.Receiver == contact && m.Sender == currentUser) || (m.Sender == contact && m.Receiver == currentUser))
                .Include(m => m.Receiver)
                .Include(m => m.Sender)
                .OrderBy(m => m.SentAt)
                .LastOrDefaultAsync();

                    conversations.Add(convo);
                }
            }

            conversations = conversations.OrderByDescending(c => c.Msg.SentAt).ToList();

            foreach (Conversation c in conversations) {
                c.Msg.SentAt = c.Msg.SentAt.ToUniversalTime();

                Console.WriteLine("-------------------------------- ");
                Console.WriteLine(c.Msg.SentAt);
                Console.WriteLine("-------------------------------- ");
            }
            //FIXME exclude private user data
            return Json(new {conversations});
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
                SentAt = DateTime.Now.ToUniversalTime()
            };
            db.Messages.Add(newMsg);
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Message sent successfully" });
        }

        [HttpPut]
        public async Task<IActionResult> MarkRead(string userId)
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
            List<Message> read = await db.Messages
                .Where(m => m.Receiver == currentUser && m.Sender == contact)
                .ToListAsync();
            foreach (Message message in read) {
                message.IsRead = true;
            }

            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Messages marked as read" });
        }
    }
}