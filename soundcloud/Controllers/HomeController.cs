using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;

namespace soundcloud.Controllers
{
    
    public class HomeController : Controller
    {
        MSHealper msh = new MSHealper();
        public ActionResult Index()
        {
            if (Session["uid"] != null)
            {
                
                DataTable dt = msh.GetTable("select id,name,email from userdata where isactive='true'");
         
                return View(dt);
            }
                
            else
                return RedirectToAction("login");

        }

        public ActionResult About()
        {
            int id = int.Parse(Request.QueryString["ID"]);
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult logout()
        {
            Session["uid"] = null;
            return RedirectToAction("Index");
        }
        public ActionResult login()
        {

            if (Session["uid"] == "-1")
            {
                ViewBag.Message = "!pass";
            }
            if (Session["uid"] == "$")
            {
                ViewBag.Message = "$";
            }
            return View();
        }
        [HttpPost]
        public ActionResult validate(string txtuname, string txtpass)
        {
            string constr = ConfigurationManager.ConnectionStrings["connlocal"].ToString();
            MySqlConnection conn = new MySqlConnection(constr);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM `userdata` WHERE `email`='" + txtuname + "' and `password`='" + txtpass + "'", conn);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count != 0)
            {
                Session["uid"] = dt.Rows[0][0].ToString();
                return RedirectToAction("Index");
            }
            else
            {
                Session["uid"] = "-1";
                return RedirectToAction("login");
            }

        }
        [HttpPost]
        public ActionResult signup(string txtuname, string txtemail, string txtpassword)
        {
            string constr = ConfigurationManager.ConnectionStrings["conns"].ToString();
            MySqlConnection conn = new MySqlConnection(constr);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("INSERT INTO `userdata`( `name`, `email`, `password`,`isactive`) VALUES ('" + txtuname + "','" + txtemail + "','" + txtpassword + "','True')", conn);
            int i = cmd.ExecuteNonQuery();
            if (i > 0)
            {
                Session["uid"] = "$";

            }
            return RedirectToAction("login");
        }
    }
}