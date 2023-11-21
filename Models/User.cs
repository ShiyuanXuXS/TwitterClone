using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TwitterClone.Models
{
    public class User:IdentityUser
    {
        public string? NickName{get;set;}
        public string? Avatar{get;set;}
        public DateTime? DateOfBirth{get;set;}
        public string? Description{get;set;}
    }
}