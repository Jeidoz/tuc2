using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tuc2.Entities;

namespace tuc2
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<TestTask> Tasks{ get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Test> Tests { get; set; } 
    }
}
