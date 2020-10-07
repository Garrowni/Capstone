using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
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
                logIn.ConfirmPassword = "TestPass";
                logIn.UserPhoto = "/UserImages/default-user-image.png";
                logIn.Active = "Yes";
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
            var volunteer = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Email == email && x.Password==password && x.Active=="Yes");
            if (volunteer==null)
            {
                ViewBag.Message="Your Email or Password is NOT correct!!! Contact The Administrator to fix it... ";
                return View();
            }
            else
            {
                var volunteerName = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Email == email && x.Password == password && x.Active == "Yes");
                
                Session["Username"] = volunteerName.Name;

                Session["UserId"] = volunteerName.Id;
                Session["Pass"] = volunteerName.Password;
                Session["Email"] = volunteerName.Email;
                Session["Img"]= volunteerName.UserPhoto.ToString();
                Session["Role"] = volunteerName.Role;
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
