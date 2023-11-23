using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TwitterClone.Models;

namespace TwitterClone.Data
{
    public class TwitterCloneDbContext : IdentityDbContext<User>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Tweet> Tweets { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Like> Likes { get; set; }
        public TwitterCloneDbContext(DbContextOptions<TwitterCloneDbContext> options) : base(options) { }



    }
}