﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Card_Collection_Tool.Models; // Imports the Card and Collection models

namespace Card_Collection_Tool.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserCardCollection> UserCardCollections { get; set; }
    }
}