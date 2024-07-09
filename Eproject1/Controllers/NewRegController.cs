using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Eproject1.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace Eproject1.Controllers
{
    public class NewRegController : Controller
    {
        // GET: NewReg
        [HttpGet]
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(UserClass uc, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(uc);
            }

            try
            {
                string mainconn = ConfigurationManager.ConnectionStrings["Myconnection"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(mainconn))
                {
                    string sqlquery = "insert into [dbo].[MVCregUser] (Uname,Uemail,Upwd,Department,Uimg) values (@Uname, @Uemail,@Upwd,@Department,@Uimg)";
                    SqlCommand sqlcomm = new SqlCommand(sqlquery, sqlConnection);
                    sqlConnection.Open();

                    sqlcomm.Parameters.AddWithValue("@Uname", uc.Uname);
                    sqlcomm.Parameters.AddWithValue("@Uemail", uc.Uemail);
                    sqlcomm.Parameters.AddWithValue("@Upwd", uc.Upwd);
                    sqlcomm.Parameters.AddWithValue("@Department", (int)uc.Department);

                    if (file != null && file.ContentLength > 0)
                    {
                        string filename = Path.GetFileName(file.FileName);
                        string imgpath = Path.Combine(Server.MapPath("~/User-Images/"), filename);
                        file.SaveAs(imgpath);
                        sqlcomm.Parameters.AddWithValue("@Uimg", "~/User-Images/" + file.FileName);
                    }
                    else
                    {
                        sqlcomm.Parameters.AddWithValue("@Uimg", DBNull.Value);
                    }

                    sqlcomm.ExecuteNonQuery();
                }

                ViewData["Message"] = "User Record " + uc.Uname + " is registered successfully";
            }
            catch (Exception ex)
            {
                ViewData["Message"] = "An error occurred: " + ex.Message;
            }

            return View(uc);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            try
            {
                string mainconn = ConfigurationManager.ConnectionStrings["Myconnection"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(mainconn))
                {
                    string sqlquery = "SELECT COUNT(1) FROM [dbo].[MVCregUser] WHERE Uemail = @Uemail AND Upwd = @Upwd";
                    SqlCommand sqlcomm = new SqlCommand(sqlquery, sqlConnection);
                    sqlcomm.Parameters.AddWithValue("@Uemail", login.Uemail);
                    sqlcomm.Parameters.AddWithValue("@Upwd", login.Upwd);
                    sqlConnection.Open();
                    int count = Convert.ToInt32(sqlcomm.ExecuteScalar());

                    if (count == 1)
                    {
                        // Successfully authenticated
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(login);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
                return View(login);
            }
        }
    }
}
