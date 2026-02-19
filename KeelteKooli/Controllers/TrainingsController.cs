using KeelteKooli.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace KeelteKooli.Controllers
{
    public class TrainingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Список всех тренингов
        public ActionResult Index()
        {
            var trainings = db.Trainings
                .Include(t => t.Keelekursus)
                .Include(t => t.Opetaja)
                .ToList();
            return View(trainings);
        }

        // Детали тренинга
        public ActionResult Details(int id)
        {
            var training = db.Trainings
                .Include(t => t.Keelekursus)
                .Include(t => t.Opetaja)
                .FirstOrDefault(t => t.Id == id);

            if (training == null) return HttpNotFound();

            return View(training);
        }
    }
}
