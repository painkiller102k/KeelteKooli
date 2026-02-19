    using KeelteKooli.Models;
    using Microsoft.AspNet.Identity;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;

    namespace KeelteKooli.Controllers
    {
        [Authorize(Roles = "Admin")]
        public class AdminController : Controller
        {
            private ApplicationDbContext db = new ApplicationDbContext();

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
                if (ModelState.IsValid)
                {
                    db.Courses.Add(course);
                    db.SaveChanges();
                    return RedirectToAction("Courses");
                }
                return View("Courses/Create", course);
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
                if (ModelState.IsValid)
                {
                    db.Entry(course).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Courses");
                }
                return View("Courses/Edit", course);
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
                db.Courses.Remove(course);
                db.SaveChanges();
                return RedirectToAction("Courses");
            }

            // ---------------- TEACHERS ----------------
            public ActionResult Teachers()
            {
                var teachers = db.Teachers.ToList();
                return View("Opetaja/Index", teachers);
            }

            public ActionResult CreateTeacher()
            {
                return View("Opetaja/Create");
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult CreateTeacher(Teacher teacher)
            {
                if (ModelState.IsValid)
                {
                    db.Teachers.Add(teacher);
                    db.SaveChanges();

                    // Создаём аккаунт для преподавателя
                    var userManager = new Microsoft.AspNet.Identity.UserManager<ApplicationUser>(
                        new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(db));
                    var newUser = new ApplicationUser
                    {
                        UserName = teacher.Email,
                        Email = teacher.Email
                    };
                    userManager.Create(newUser, "Teacher123!");
                    userManager.AddToRole(newUser.Id, "Opetaja");
                    teacher.ApplicationUserId = newUser.Id;
                    db.SaveChanges();

                    return RedirectToAction("Teachers");
                }
                return View("Opetaja/Create", teacher);
            }

            // Edit/Delete для Teachers делается аналогично Courses

            // ---------------- TRAININGS ----------------
            public ActionResult Trainings()
            {
                var trainings = db.Trainings.ToList();
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
                if (ModelState.IsValid)
                {
                    db.Trainings.Add(training);
                    db.SaveChanges();
                    return RedirectToAction("Trainings");
                }
                ViewBag.Courses = new SelectList(db.Courses, "Id", "Nimetus", training.KeelekursusId);
                ViewBag.Teachers = new SelectList(db.Teachers, "Id", "Nimi", training.OpetajaId);
                return View("Trainings/Create", training);
            }

            // Edit/Delete для Trainings аналогично Courses
        }
    }
