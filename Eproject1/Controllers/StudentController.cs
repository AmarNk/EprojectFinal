using Eproject1.Models;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

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

                    ModelState.Clear();


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
    
        public ActionResult StudentList()
        {

           var complaintList = dbObj.tbl_Student.ToList();
           return View(complaintList); 
        }

        public ActionResult Edit(int stid)
        {
            var row = dbObj.tbl_Student.Where(model => model.Sid == stid).FirstOrDefault();
            return View(row);
        }

        [HttpPost]

        public ActionResult Edit(tbl_Student s)
        {
            if (ModelState.IsValid == true)
            {
                dbObj.Entry(s).State = EntityState.Modified;
                int a = dbObj.SaveChanges();
                if (a > 0) 
                {
                    TempData["msg"] = "<script>alert('Data updated successfully')</script>";
                    return RedirectToAction("StudentList");
                }
                else
                {
                    TempData["msg"] = "<script>alert('Data not updated')</script>";
                }
            }
            return View(s);
        }


        public ActionResult Delete(int stid)
        {
            if (stid > 0)
            {
                var rowdata = dbObj.tbl_Student.Where(model => model.Sid == stid).FirstOrDefault();

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

            return RedirectToAction("StudentList");
        }








    }
}

