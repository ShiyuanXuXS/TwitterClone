using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterClone.Models
{
    public class Follow
    {
        public User User{get;set;}
        public User Author{get;set;}
        public DateTime CreatedAt{get;set;}
    }
}