using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
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
            var vol = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Email == email);
            if (vol==null)
            {
                ViewBag.Message = "Click Register to register yourself and wait for Admin Approval !!!";
                return View();
            }
            else
            {
                var voll = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Email == email && x.Password == password && x.Active == "Yes");

                var volunteer = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Email == email && x.Password == password && x.Active == "No");
                if (volunteer != null)
                {
                    ViewBag.Message = "Wait for Admin Approval !!!";
                    return View();
                }
                else
                {
                    if (voll == null)
                    {
                        ViewBag.Message = "Incorrect Password!!!";
                        return View();
                    }
                    else
                    {
                        var volunteerName = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Email == email && x.Password == password && x.Active == "Yes");

                        Session["Username"] = volunteerName.Name;

                        Session["UserId"] = volunteerName.Id;
                      //  Session["Pass"] = volunteerName.Password;
                        Session["Email"] = volunteerName.Email;
                        Session["Img"] = volunteerName.UserPhoto.ToString();
                        Session["Role"] = volunteerName.Role;
                        return View("../Home/Index");
                    }
                }
               
            }
           
           
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("LogIn","LogIn");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string id,LogInModel logIn)
        {
            var email = logIn.Email;
            var volunteer = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Email == email );
            if (volunteer==null)
            {
                ViewBag.Message = "Your are not Registered... Please Register First!!!! ";
            }
            else
            {
                var filter = Builders<VolunteerModel>.Filter.Eq("_id", volunteer.Id);
                string password = Membership.GeneratePassword(12, 1);
                var update = Builders<VolunteerModel>.Update
                   .Set("Password", password)
                   .Set("ConfirmPassword", password);

                var result = volunteerCollection.UpdateOne(filter, update);
                MailAddressModel mode = new MailAddressModel();
                mode.To = email;
                mode.From = "testint65@gmail.com";
                mode.Subject = "Hello";
                mode.Body = "Hello, use the following temporary password to log in and make sure you change it as soon as you can: <h2>"+password+"</h2> ";
                
                SendEmail(mode);
                
                ViewBag.Message = "Email was sent to you with a temporary password....";
            }
            return View();
        }

        public void SendEmail(MailAddressModel model)
        {
            MailMessage mail = new MailMessage();

            mail.To.Add(model.To);
            mail.From = new MailAddress(model.From);
            mail.Subject = model.Subject;
            string Body = model.Body;
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("testint65@gmail.com", "TestingPass"); // Enter seders User name and password  
            smtp.EnableSsl = true;
            smtp.Send(mail);

        }
    }
}
