using KeelteKooli.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace KeelteKooli.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // ---------------- COURSES ----------------
        public ActionResult Courses()
        {
            var courses = db.Courses.ToList();
            return View("Courses/Index", courses);
        }

        public ActionResult CreateCourse()
        {
            return View("Courses/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCourse(Course course)
        {
            if (!ModelState.IsValid)
                return View("Courses/Create", course);

            db.Courses.Add(course);
            db.SaveChanges();
            return RedirectToAction(nameof(Courses));
        }

        public ActionResult EditCourse(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var course = db.Courses.Find(id);
            if (course == null) return HttpNotFound();

            return View("Courses/Edit", course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCourse(Course course)
        {
            if (!ModelState.IsValid)
                return View("Courses/Edit", course);

            db.Entry(course).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction(nameof(Courses));
        }

        public ActionResult DeleteCourse(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var course = db.Courses.Find(id);
            if (course == null) return HttpNotFound();

            return View("Courses/Delete", course);
        }

        [HttpPost, ActionName("DeleteCourse")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCourseConfirmed(int id)
        {
            var course = db.Courses.Find(id);
            if (course == null) return HttpNotFound();

            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction(nameof(Courses));
        }

        // ---------------- TEACHERS (OPETAJA) ----------------
        public ActionResult Teachers()
        {
            var teachers = db.Teachers.Include(t => t.User).ToList();
            return View("Opetaja/Index", teachers);
        }

        public ActionResult CreateTeacher()
        {
            return View("Opetaja/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTeacher(Teacher teacher, string Email, string Password)
        {
            if (string.IsNullOrWhiteSpace(Email))
                ModelState.AddModelError("Email", "Email is required");
            if (string.IsNullOrWhiteSpace(Password))
                ModelState.AddModelError("Password", "Password is required");

            if (!ModelState.IsValid)
                return View("Opetaja/Create", teacher);

            // создаём identity user
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(db));

            var newUser = new ApplicationUser
            {
                UserName = Email,
                Email = Email
            };

            var result = userManager.Create(newUser, Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", string.Join(", ", result.Errors));
                return View("Opetaja/Create", teacher);
            }

            userManager.AddToRole(newUser.Id, "Opetaja");

            // сохраняем teacher + связь с ApplicationUser
            teacher.Email = Email;
            teacher.ApplicationUserId = newUser.Id;

            db.Teachers.Add(teacher);
            db.SaveChanges();

            return RedirectToAction(nameof(Teachers));
        }

        public ActionResult EditTeacher(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var teacher = db.Teachers.Find(id);
            if (teacher == null) return HttpNotFound();

            return View("Opetaja/Edit", teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTeacher(Teacher teacher)
        {
            if (!ModelState.IsValid)
                return View("Opetaja/Edit", teacher);

            // если ты не хочешь менять Email / ApplicationUserId при редактировании:
            db.Entry(teacher).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction(nameof(Teachers));
        }

        public ActionResult DeleteTeacher(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var teacher = db.Teachers.Include(t => t.User).FirstOrDefault(t => t.Id == id);
            if (teacher == null) return HttpNotFound();

            return View("Opetaja/Delete", teacher);
        }

        [HttpPost, ActionName("DeleteTeacher")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTeacherConfirmed(int id)
        {
            var teacher = db.Teachers.Find(id);
            if (teacher == null) return HttpNotFound();

            // удалить все trainings этого учителя (иначе FK ошибка)
            var trainings = db.Trainings.Where(t => t.OpetajaId == teacher.Id).ToList();
            if (trainings.Any())
                db.Trainings.RemoveRange(trainings);

            // удалить Identity user
            if (!string.IsNullOrEmpty(teacher.ApplicationUserId))
            {
                var user = db.Users.Find(teacher.ApplicationUserId);
                if (user != null)
                    db.Users.Remove(user);
            }

            db.Teachers.Remove(teacher);
            db.SaveChanges();

            return RedirectToAction(nameof(Teachers));
        }

        // ---------------- TRAININGS ----------------
        public ActionResult Trainings()
        {
            var trainings = db.Trainings
                .Include(t => t.Keelekursus)
                .Include(t => t.Opetaja)
                .ToList();

            return View("Trainings/Index", trainings);
        }

        public ActionResult CreateTraining()
        {
            ViewBag.Courses = new SelectList(db.Courses, "Id", "Nimetus");
            ViewBag.Teachers = new SelectList(db.Teachers, "Id", "Nimi");
            return View("Trainings/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTraining(Training training)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Courses = new SelectList(db.Courses, "Id", "Nimetus", training.KeelekursusId);
                ViewBag.Teachers = new SelectList(db.Teachers, "Id", "Nimi", training.OpetajaId);
                return View("Trainings/Create", training);
            }

            db.Trainings.Add(training);
            db.SaveChanges();
            return RedirectToAction(nameof(Trainings));
        }

        public ActionResult EditTraining(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var training = db.Trainings.Find(id);
            if (training == null) return HttpNotFound();

            ViewBag.Courses = new SelectList(db.Courses, "Id", "Nimetus", training.KeelekursusId);
            ViewBag.Teachers = new SelectList(db.Teachers, "Id", "Nimi", training.OpetajaId);
            return View("Trainings/Edit", training);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTraining(Training training)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Courses = new SelectList(db.Courses, "Id", "Nimetus", training.KeelekursusId);
                ViewBag.Teachers = new SelectList(db.Teachers, "Id", "Nimi", training.OpetajaId);
                return View("Trainings/Edit", training);
            }

            db.Entry(training).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction(nameof(Trainings));
        }

        public ActionResult DeleteTraining(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var training = db.Trainings
                .Include(t => t.Keelekursus)
                .Include(t => t.Opetaja)
                .FirstOrDefault(t => t.Id == id);

            if (training == null) return HttpNotFound();

            return View("Trainings/Delete", training);
        }

        [HttpPost, ActionName("DeleteTraining")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTrainingConfirmed(int id)
        {
            var training = db.Trainings.Find(id);
            if (training == null) return HttpNotFound();

            db.Trainings.Remove(training);
            db.SaveChanges();
            return RedirectToAction(nameof(Trainings));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}