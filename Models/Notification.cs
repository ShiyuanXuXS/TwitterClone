using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterClone.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public User? User { get; set; }
        public int Status { get; set; }  //0:unread, 1:read
    }
}