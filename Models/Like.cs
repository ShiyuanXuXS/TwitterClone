using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterClone.Models
{
    public class Like
    {
        public User User{get;set;}
        public Tweet Tweet{get;set;}
        public DateTime CreatedAt{get;set;}
    }
}