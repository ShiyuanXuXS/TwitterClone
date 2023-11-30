using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace TwitterClone.Pages
{
    [Authorize]
    public class MessagesModel : PageModel
    {

        private readonly UserManager<User> userManager;
        private readonly ILogger<MessagesModel> logger;
        private readonly TwitterCloneDbContext db;

        public List<User> Contacts { get; set; }
        public List<Message> Sent { get; set; }

        public List<Message> Received { get; set; }

        public List<Conversation> Conversations { get; set; }
        public class Conversation
        {
            public User Contact { get; set; }
            public List<Message> Messages { get; set; }
        }


        public MessagesModel(UserManager<User> userManager, ILogger<MessagesModel> logger, TwitterCloneDbContext db)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.db = db;
        }

        public async Task OnGetAsync()
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var id = currentUser.Id;

            Sent = await db.Messages
            .Where(m => m.Sender.Id == id)
            .Include(m => m.Receiver)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

            Received = await db.Messages
            .Where(m => m.Receiver.Id == id)
            .OrderByDescending(m => m.SentAt)
            .Include(m => m.Sender)
            .ToListAsync();
            Conversations = new List<Conversation>();
            Contacts = new List<User>();
            foreach (Message msg in Sent)
            {
                if (!Contacts.Contains(msg.Receiver))
                {
                    Contacts.Add(msg.Receiver);
                                        Console.WriteLine("--------------");
                                        Console.WriteLine(msg.Receiver.UserName);

                                        Console.WriteLine("--------------");
                }
            }
            foreach (Message msg in Received)
            {
                if (!Contacts.Contains(msg.Sender))
                {
                    Contacts.Add(msg.Sender);
                    Console.WriteLine("--------------");
                                        Console.WriteLine(msg.Sender.UserName);

                                        Console.WriteLine("--------------");

                }
            }
            if (Contacts.Any() == true)
            {
                foreach (User contact in Contacts)
                {
                    Conversation convo = new Conversation();
                    convo.Contact = contact;
                    convo.Messages = await db.Messages
                .Where(m => (m.Receiver == contact && m.Sender == currentUser) || (m.Sender == contact && m.Receiver == currentUser))
                .Include(m => m.Receiver)
                .Include(m => m.Sender)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
                    Conversations.Add(convo);
                }
            }

        }


    }
}
