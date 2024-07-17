using System.Linq;
using System.Web.Mvc;
using System.Data.SqlClient;
using Eproject1.Models;
using System.Data.Entity.Validation;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity;

namespace Eproject1.Controllers
{
    public class AdminController : Controller
    {
        private AdminContext db = new AdminContext();
        private online_help_deskEntities1 dbObj = new online_help_deskEntities1();

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

        public ActionResult Addfaculty()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Addfaculty(tbl_faculty model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                tbl_faculty obj = new tbl_faculty
                {
                    fname = model.fname,
                    femail = model.femail,
                    Department = model.Department,
                    JobResponsibility = model.JobResponsibility,
                    fpassword = model.fpassword
                };

                dbObj.tbl_faculty.Add(obj);
                dbObj.SaveChanges();

                return RedirectToAction("facultyList"); // Redirect to a view that lists faculties, adjust as needed
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return View(model); // Return the view with errors
            }
        }


        public ActionResult facultyList()
        {
            var flist = dbObj.tbl_faculty.ToList();

            return View(flist);
        }

        public ActionResult Delete(int flid)
        {
            if (flid > 0)
            {
                var rowdata = dbObj.tbl_faculty.Where(model => model.fid == flid).FirstOrDefault();

                if (rowdata != null)
                {
                    dbObj.Entry(rowdata).State = EntityState.Deleted;
                    int a = dbObj.SaveChanges();
                    if (a > 0)
                    {
                        TempData["msg"] = "<script>alert('Data deleted')</script>";
                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('Data not found')</script>";
                    }
                }
            }

            return RedirectToAction("facultyList");
        }

    }
}
