using KeelteKooli.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace KeelteKooli.Controllers
{
    [Authorize(Roles = "Opetaja")]
    public class OpetajaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Dashboard преподавателя
        public ActionResult Dashboard()
        {
            var userId = User.Identity.GetUserId();
            var teacher = db.Teachers.FirstOrDefault(t => t.ApplicationUserId == userId);
            if (teacher == null) return HttpNotFound();

            var trainings = db.Trainings
                .Include(t => t.Keelekursus)
                .Where(t => t.OpetajaId == teacher.Id)
                .ToList();

            return View(trainings);
        }

        // Список студентов на тренингах преподавателя
        public ActionResult Students(int trainingId)
        {
            var registrations = db.Registrations
                .Include(r => r.User)
                .Include(r => r.Koolitus.Keelekursus)
                .Where(r => r.KoolitusId == trainingId)
                .ToList();

            return View(registrations);
        }
    }
}
