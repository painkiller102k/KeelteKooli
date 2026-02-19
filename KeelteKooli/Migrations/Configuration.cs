using KeelteKooli.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace KeelteKooli.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<KeelteKooli.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(KeelteKooli.Models.ApplicationDbContext context)
        {
            // ---------------- ROLES ----------------
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            string[] roles = { "Admin", "Opetaja", "Opilane" };

            foreach (var roleName in roles)
            {
                if (!roleManager.RoleExists(roleName))
                {
                    roleManager.Create(new IdentityRole(roleName));
                }
            }

            // ---------------- ADMIN ----------------
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            string adminEmail = "admin@gmail.com";
            var adminUser = userManager.FindByEmail(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };
                userManager.Create(adminUser, "Admin123!");
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            // ---------------- TEACHER ----------------
            string teacherEmail = "teacher@gmail.com";
            var teacherUser = userManager.FindByEmail(teacherEmail);

            if (teacherUser == null)
            {
                teacherUser = new ApplicationUser
                {
                    UserName = teacherEmail,
                    Email = teacherEmail
                };
                userManager.Create(teacherUser, "Teacher123!");
                userManager.AddToRole(teacherUser.Id, "Opetaja");
            }

            if (!context.Teachers.Any(t => t.ApplicationUserId == teacherUser.Id))
            {
                context.Teachers.Add(new Teacher
                {
                    Nimi = "Irina Merkulova",
                    Kvalifikatsioon = "English C1",
                    FotoPath = "teacher1.jpg",
                    ApplicationUserId = teacherUser.Id
                });
            }

            // ---------------- COURSES ----------------
            if (!context.Courses.Any())
            {
                var course = new Course
                {
                    Nimetus = "Английский для начинающих",
                    Keel = "English",
                    Tase = "A1",
                    Kirjeldus = "Базовый курс английского языка"
                };
                context.Courses.Add(course);
                context.SaveChanges();

                var teacher = context.Teachers.FirstOrDefault();
                if (teacher != null)
                {
                    context.Trainings.Add(new Training
                    {
                        KeelekursusId = course.Id,
                        OpetajaId = teacher.Id,
                        AlgusKuupaev = DateTime.Today,
                        LoppKuupaev = DateTime.Today.AddMonths(1),
                        Hind = 150,
                        MaxOsalejaid = 10
                    });
                }
            }

            context.SaveChanges();
        }
    }
}
