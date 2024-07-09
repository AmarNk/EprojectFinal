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
            try
            {
                string mainconn = ConfigurationManager.ConnectionStrings["Myconnection"].ConnectionString;
                using (SqlConnection sqlConnection = new SqlConnection(mainconn))
                {
                    string sqlquery = "insert into [dbo].[MVCregUser] (Uname,Uemail,Upwd,Department,Uimg) values (@Uname, @Uemail,@Upwd,@Department,@Uimg)";
                    SqlCommand sqlcomm = new SqlCommand(sqlquery, sqlConnection);
                    sqlConnection.Open();

                    if (!string.IsNullOrEmpty(uc.Uname))
                    {
                        sqlcomm.Parameters.AddWithValue("@Uname", uc.Uname);
                    }
                    else
                    {
                        throw new Exception("Uname is required");
                    }

                    if (!string.IsNullOrEmpty(uc.Uemail))
                    {
                        sqlcomm.Parameters.AddWithValue("@Uemail", uc.Uemail);
                    }
                    else
                    {
                        throw new Exception("Uemail is required");
                    }

                    if (!string.IsNullOrEmpty(uc.Upwd))
                    {
                        sqlcomm.Parameters.AddWithValue("@Upwd", uc.Upwd);
                    }
                    else
                    {
                        throw new Exception("Upwd is required");
                    }

                    if (!string.IsNullOrEmpty(uc.Department))
                    {
                        sqlcomm.Parameters.AddWithValue("@Department", uc.Department);
                    }
                    else
                    {
                        sqlcomm.Parameters.AddWithValue("@Department", DBNull.Value);
                    }

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
                // Log the exception (ex) here or handle it as needed
                ViewData["Message"] = "An error occurred: " + ex.Message;
            }

            return View();
        }
    }
}
