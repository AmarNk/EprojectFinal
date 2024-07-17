using Eproject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eproject1.Controllers
{
    public class AssignController : Controller
    {
        online_help_deskEntities2 db = new online_help_deskEntities2();
        // GET: Assign
        public ActionResult Assign()
        {
            var students = db.tbl_Student.ToList();
            var faculties = db.tbl_faculty.ToList();
            ViewBag.Faculties = new SelectList(faculties, "fid", "fname");
            return View(students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(int Sid, int fid)
        {
            var student = db.tbl_Student.Find(Sid);
            if (student == null)
            {
                return HttpNotFound();
            }

            var faculty = db.tbl_faculty.Find(fid);
            if (faculty == null)
            {
                return HttpNotFound();
            }

            var complaintAssign = new tbl_complaint_assign
            {
                Sid = student.Sid,
                studentname = student.Sname,
                facultyname = faculty.fname,
                email = student.Email,
                //mobile = student.Mobile,I have mistakenly in assign table give it datatype as varchar while in tbl_student its datatype is int
                sdepartment = student.Department,
                semester = student.Semester,
                program = student.Program,
                complaint_against = student.ComplaintAgainst,
                Description = student.Description,
                facultyresponsibility = faculty.JobResponsibility,
            };

            db.tbl_complaint_assign.Add(complaintAssign);
            db.SaveChanges();



            return View("Assign");        }
    }
}