using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterClone.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string? Body { get; set; }
        public User? Commenter { get; set; }
        public Tweet? Tweet { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Suspended { get; set; }
    }
}