using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;
using TermProjectUI.App_Start;
using TermProjectUI.Models;

namespace TermProjectUI.Controllers
{
    public class LogInController : Controller
    {
        private MongoDBContext dbcontext;
        private IMongoCollection<VolunteerModel> volunteerCollection;

        public LogInController()
        {
            dbcontext = new MongoDBContext();
            volunteerCollection = dbcontext.database.GetCollection<VolunteerModel>("volunteer");

        }
        // GET: LogIn
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LogInModel logIn)
        {
            var email =logIn.Email;
            var password = logIn.Password;
            var volunteer = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Email == email && x.Password==password);
            if (volunteer==null)
            {
                ViewBag.Message="Contact me";
                return View();
            }
            else
            {
                return View("../Home/Index");
            }
           
        }

    }
}
