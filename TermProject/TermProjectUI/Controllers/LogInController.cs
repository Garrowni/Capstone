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
        static List<string> volunteerList = new List<string>();

        static List<string> currentTasks = new List<string>();
        static List<string> currentJoinedTasks = new List<string>();
        private IMongoCollection<TransportationTaskModel> transportationCollection;
        private IMongoCollection<InventoryTaskModel> inventoryCollection;
        private IMongoCollection<PhotographyTaskModel> photographyCollection;
        private IMongoCollection<GroomingTaskModel> groomingCollection;
        private IMongoCollection<VetTaskModel> vetCollection;
        private IMongoCollection<OtherTaskModel> otherCollection;
        public LogInController()
        {
            dbcontext = new MongoDBContext();
            volunteerCollection = dbcontext.database.GetCollection<VolunteerModel>("volunteer");

            transportationCollection = dbcontext.database.GetCollection<TransportationTaskModel>("transportation");
            inventoryCollection = dbcontext.database.GetCollection<InventoryTaskModel>("inventory");
            photographyCollection = dbcontext.database.GetCollection<PhotographyTaskModel>("photography");
            groomingCollection = dbcontext.database.GetCollection<GroomingTaskModel>("grooming");
            vetCollection = dbcontext.database.GetCollection<VetTaskModel>("vet");
            otherCollection = dbcontext.database.GetCollection<OtherTaskModel>("other");

        }
        // GET: LogIn
        public ActionResult LogIn()
        {
            currentTasks = new List<string>();

            List<TransportationTaskModel> products = transportationCollection.AsQueryable<TransportationTaskModel>().ToList();
            List<InventoryTaskModel> inventory = inventoryCollection.AsQueryable<InventoryTaskModel>().ToList();
            List<PhotographyTaskModel> photography = photographyCollection.AsQueryable<PhotographyTaskModel>().ToList();
            List<GroomingTaskModel> grooming = groomingCollection.AsQueryable<GroomingTaskModel>().ToList();
            List<VetTaskModel> vet = vetCollection.AsQueryable<VetTaskModel>().ToList();
            List<OtherTaskModel> others = otherCollection.AsQueryable<OtherTaskModel>().ToList();

            foreach (var product in products)
            {
                currentTasks.Add(product.Id.ToString());

            }
            foreach (var groom in grooming)
            {
                currentTasks.Add(groom.Id.ToString());

            }
            foreach (var inv in inventory)
            {
                currentTasks.Add(inv.Id.ToString());

            }
            foreach (var photo in photography)
            {
                currentTasks.Add(photo.Id.ToString());

            }
            foreach (var v in vet)
            {
                currentTasks.Add(v.Id.ToString());

            }
            foreach (var ot in others)
            {
                currentTasks.Add(ot.Id.ToString());

            }

            Session["TaskCount"] = currentTasks.Count().ToString();

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
            List<VolunteerModel> volunteers = volunteerCollection.AsQueryable<VolunteerModel>().ToList();

            volunteerList = new List<string>();
           
            foreach (var volun in volunteers)
            {
                if (volun.Active=="Yes")
                {
                    volunteerList.Add(volun.Name.ToString());

                }
                
            }
            Session["VolunteerList"] = volunteerList;
           
            var email =logIn.Email;
            var password = logIn.Password;
            var vol = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Email == email);

            List<TransportationTaskModel> products = transportationCollection.AsQueryable<TransportationTaskModel>().ToList();
            List<InventoryTaskModel> inventory = inventoryCollection.AsQueryable<InventoryTaskModel>().ToList();
            List<PhotographyTaskModel> photography = photographyCollection.AsQueryable<PhotographyTaskModel>().ToList();
            List<GroomingTaskModel> grooming = groomingCollection.AsQueryable<GroomingTaskModel>().ToList();
            List<VetTaskModel> vet = vetCollection.AsQueryable<VetTaskModel>().ToList();
            List<OtherTaskModel> others = otherCollection.AsQueryable<OtherTaskModel>().ToList();

            currentJoinedTasks = new List<string>();

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

                        foreach (var trans in products)
                        {
                            if (trans.assignees != null)
                            {
                                foreach (var assignee in trans.assignees)
                                {
                                    if (assignee.ToString() == volunteerName.Id.ToString())
                                    {
                                        currentJoinedTasks.Add(trans.Id.ToString());
                                    }
                                }
                            }


                        }
                        foreach (var inv in inventory)
                        {
                            if (inv.assignees != null)
                            {
                                foreach (var assignee in inv.assignees)
                                {
                                    if (assignee.ToString() == volunteerName.Id.ToString())
                                    {
                                        currentJoinedTasks.Add(inv.Id.ToString());
                                    }
                                }
                            }


                        }
                        foreach (var photo in photography)
                        {
                            if (photo.assignees != null)
                            {
                                foreach (var assignee in photo.assignees)
                                {
                                    if (assignee.ToString() == volunteerName.Id.ToString())
                                    {
                                        currentJoinedTasks.Add(photo.Id.ToString());
                                    }
                                }
                            }

                        }
                        foreach (var groom in grooming)
                        {
                            if (groom.assignees != null)
                            {
                                foreach (var assignee in groom.assignees)
                                {
                                    if (assignee.ToString() == volunteerName.Id.ToString())
                                    {
                                        currentJoinedTasks.Add(groom.Id.ToString());
                                    }
                                }
                            }

                        }
                        foreach (var v in vet)
                        {
                            if (v.assignees != null)
                            {
                                foreach (var assignee in v.assignees)
                                {
                                    if (assignee.ToString() == volunteerName.Id.ToString())
                                    {
                                        currentJoinedTasks.Add(v.Id.ToString());
                                    }
                                }
                            }

                        }
                        foreach (var other in others)
                        {
                            if (other.assignees != null)
                            {
                                foreach (var assignee in other.assignees)
                                {
                                    if (assignee.ToString() == volunteerName.Id.ToString())
                                    {
                                        currentJoinedTasks.Add(other.Id.ToString());
                                    }
                                }
                            }

                        }

                        Session["JoinedTaskCount"] = currentJoinedTasks.Count().ToString();



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
                
                GenerateNewPassword(mode);
                
                ViewBag.Message = "Email was sent to you with a temporary password....";
            }
            return View();
        }

        public void GenerateNewPassword(MailAddressModel model)
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
