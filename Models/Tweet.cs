using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterClone.Models
{
    public class Tweet
    {
        public int Id{get;set;}
        public string? Title{get;set;}
        public string? Body{get;set;}
        public Tweet? ParentTweet{get;set;}
        public User? Author{get;set;}
        public DateTime CreatedAt{get;set;}
        public bool Deleted{get;set;}
        public bool Suspended{get;set;}
    }
}