using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TwitterClone.Models
{
    [PrimaryKey(nameof(UserId), nameof(TweetId))]
    public class Like
    {
        // [Key, Column(Order = 1)]
        public string UserId{get;set;}

        // [Key, Column(Order = 2)]
        public int TweetId { get; set; }
        public DateTime CreatedAt{get;set;}


        public User User{get;set;}
        public Tweet Tweet{get;set;}

        
    }
}