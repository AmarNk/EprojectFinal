using System.Linq;
using System.Web.Mvc;
using Eproject1.Models;

namespace Eproject1.Controllers
{
    public class AdminController : Controller
    {
        private AdminContext db = new AdminContext();

        // GET: Admin/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Admin admin)
        {
            if (!ModelState.IsValid)
            {
                return View(admin); // Return the view with validation errors
            }

            var adminInDb = db.Admins.FirstOrDefault(a => a.Email == admin.Email && a.Password == admin.Password);

            if (adminInDb != null)
            {
                // Successfully authenticated
                Session["AdminEmail"] = admin.Email;
                return RedirectToAction("StudentList", "Student"); // Redirect to StudentList action in Student controller
            }
            else
            {
                // Invalid email or password
                ModelState.AddModelError("", "Invalid email or password.");
                return View(admin); // Return the view with errors
            }

        }
    }
}
