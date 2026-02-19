using KeelteKooli.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace KeelteKooli.Controllers
{
    [Authorize(Roles = "Opilane")]
    public class StudentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Мои тренинги
        public ActionResult MyTrainings()
        {
            var userId = User.Identity.GetUserId();

            var registrations = db.Registrations
                .Include(r => r.Koolitus.Keelekursus)
                .Include(r => r.Koolitus.Opetaja)
                .Where(r => r.UserId == userId)
                .ToList();

            return View(registrations);
        }

        // Регистрация на тренинг
        public ActionResult Register(int trainingId)
        {
            var userId = User.Identity.GetUserId();
            var exists = db.Registrations.Any(r => r.UserId == userId && r.KoolitusId == trainingId);
            if (!exists)
            {
                db.Registrations.Add(new Registration
                {
                    UserId = userId,
                    KoolitusId = trainingId,
                    Staatus = "Ootel"
                });
                db.SaveChanges();
                TempData["Success"] = "Вы успешно зарегистрировались на тренинг!";
            }
            else
            {
                TempData["Error"] = "Вы уже зарегистрированы на этот тренинг!";
            }
            return RedirectToAction("MyTrainings");
        }

        // Отмена регистрации
        public ActionResult Cancel(int registrationId)
        {
            var registration = db.Registrations.Find(registrationId);
            if (registration != null && registration.UserId == User.Identity.GetUserId())
            {
                db.Registrations.Remove(registration);
                db.SaveChanges();
                TempData["Success"] = "Регистрация отменена!";
            }
            return RedirectToAction("MyTrainings");
        }
    }
}
