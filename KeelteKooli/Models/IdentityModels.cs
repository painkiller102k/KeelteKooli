using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KeelteKooli.Models
{
    // Пользователь Identity
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }

    // Контекст базы данных
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        // Добавляем DbSet для моделей приложения
        public System.Data.Entity.DbSet<Course> Courses { get; set; }
        public System.Data.Entity.DbSet<Teacher> Teachers { get; set; }
        public System.Data.Entity.DbSet<Training> Trainings { get; set; }
        public System.Data.Entity.DbSet<Registration> Registrations { get; set; }
    }
}
