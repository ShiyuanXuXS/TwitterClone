using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterClone.Models
{
    public class HubConnection
    {
        public int Id { get; set; }
        public int ConnectionId { get; set; }

        public User? User { get; set; }


    }
}