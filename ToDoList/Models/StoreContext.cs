using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ToDoList.Models
{
    public class StoreContext : DbContext
    {
        public DbSet<TableTasks> TableTasks { get; set; }
        public DbSet<User> Users { get; set; }
        public StoreContext(DbContextOptions<StoreContext> options) : base(options) 
        { 
        
        
        }
      

    }
}

