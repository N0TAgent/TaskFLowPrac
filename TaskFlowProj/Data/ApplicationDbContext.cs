using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TaskFlow.Models;

namespace TaskFlow.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
    }
}
