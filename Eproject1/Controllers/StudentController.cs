using Eproject1.Models;
using System;
using System.Data.Entity.Validation;
using System.Web.Mvc;

namespace Eproject1.Controllers
{
    public class StudentController : Controller
    {
        online_help_deskEntities dbObj = new online_help_deskEntities();

        // GET: Student
        public ActionResult Student()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddStudent(tbl_Student model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dbObj.tbl_Student.Add(model);
                    dbObj.SaveChanges();

                    return RedirectToAction("Student"); // Redirect to a success action or view
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            // Log each validation error
                            System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            ModelState.AddModelError("", $"Validation Error: {validationError.ErrorMessage}");
                        }
                    }
                }
            }

            // If ModelState is not valid, return the view with the model to display validation errors
            return View("Student", model);
        }
    }
}
