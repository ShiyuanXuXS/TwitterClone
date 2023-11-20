using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TwitterClone.Models
{
    // [PrimaryKey(nameof(UserId), nameof(AuthorId))]
    public class Follow
    {
        
        // public string UserId{get;set;}
        
        // public string AuthorId{get;set;}

        public int Id{get;set;}
        public DateTime CreatedAt{get;set;}
        

        public User User{get;set;}
        public User Author{get;set;}
        
        
    }
}