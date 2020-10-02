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
        public ActionResult LogIn()
        {
            
            List<VolunteerModel> volunteers = volunteerCollection.AsQueryable<VolunteerModel>().ToList();
            bool isEmpty = !volunteers.Any();
            if (isEmpty)
            {
               VolunteerModel logIn = new VolunteerModel();
                logIn.Email = "Test@gmail.com";
                logIn.Name = "Test";
                logIn.Role = "Admin";
                logIn.Password = "TestPass";
                volunteerCollection.InsertOne(logIn);
            }
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(LogInModel logIn)
        {
            var email =logIn.Email;
            var password = logIn.Password;
            var volunteer = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Email == email && x.Password==password);
            if (volunteer==null)
            {
                ViewBag.Message="Your Email or Password is NOT correct!!! Contact The Administrator to fix it... ";
                return View();
            }
            else
            {
                var volunteerName = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Email == email && x.Password == password);
                
                Session["Username"] = volunteerName.Name;
                return View("../Home/Index");
            }
           
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("LogIn","LogIn");
        }

    }
}
