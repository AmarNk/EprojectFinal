using System.Data.Entity;

namespace Eproject1.Models
{
    public class AdminContext : DbContext
    {
        public AdminContext() : base("Myconnection")
        {
        }

        public DbSet<Admin> Admins { get; set; }
    }
}
