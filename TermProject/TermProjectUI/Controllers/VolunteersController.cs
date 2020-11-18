using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TermProjectUI.App_Start;
using TermProjectUI.Models;

namespace TermProjectUI.Controllers
{
    
    public class VolunteersController : Controller
    {

        private MongoDBContext dbcontext;
        private IMongoCollection<VolunteerModel> volunteerCollection;
        static List<string> volunteerList = new List<string>();
        public VolunteersController()
        {
            dbcontext = new MongoDBContext();
            volunteerCollection = dbcontext.database.GetCollection<VolunteerModel>("volunteer");

        }
        // GET: Members
        public ActionResult Index()
        {
           
            // List<VolunteerModel> volunteer = volunteerCollection.AsQueryable<VolunteerModel>().ToList();
            List<VolunteerModel> volunteers = volunteerCollection.AsQueryable<VolunteerModel>().ToList();
           // List<VolunteerModel> vols = new List<VolunteerModel>();
          //  foreach (var volunteer in volunteers)
          /** {
                if (volunteer.Active=="No")
                {
                    vols.Add(volunteer);
                }
                
            }
           */

            return View(volunteers);
        }

        // GET: Members/Details/5
        public ActionResult Details(string id)
        {
            var volunteerId = new ObjectId(id);
            var volunteer = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Id == volunteerId);
            return View(volunteer);
        }

        // GET: Members/Create
        public ActionResult Create()
        {
           
            return View();
        }

        // POST: Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VolunteerModel volunteer)
        {
           
                var vol = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Email==volunteer.Email);
                if (vol==null)
                {
                    volunteer.UserPhoto = "/UserImages/default-user-image.png";
                    volunteer.Active = "No";
                    // TODO: Add insert logic here
                    volunteerCollection.InsertOne(volunteer);
                    return RedirectToAction("Details", new { id = volunteer.Id });
                }
                else
                {
                    ViewBag.Message = "The account with the entered Email Address has already been registered click Forget Password to Recover your password ";
                    return View();
                }
               
            
        }

        // GET: Members/Edit/5
        public ActionResult Edit(string id)
        {
            var volunteerId = new ObjectId(id);
            var volunteer = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Id == volunteerId);
            return View(volunteer);
        }

        // POST: Members/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, VolunteerModel volunteer)
        {
            try
            {
                // TODO: Add update logic here
                var filter = Builders<VolunteerModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<VolunteerModel>.Update
                    .Set("Name", volunteer.Name)
                    .Set("Email", volunteer.Email);
                    
                var result = volunteerCollection.UpdateOne(filter, update);
                return RedirectToAction("Details", new { id = id });
            }
            catch
            {
                return View();
            }
        }

        // GET: Members/Delete/5
        public ActionResult Delete(string id)
        {
            var volunteerId = new ObjectId(id);
            var volunteer = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Id == volunteerId);
            return View(volunteer);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                volunteerCollection.DeleteOne(Builders<VolunteerModel>.Filter.Eq("_id", ObjectId.Parse(id)));
                if (Session["Role"]==null)
                {
                    return RedirectToAction("../LogIn/LogIn");
                }
                else
                {
                    return RedirectToAction("../Volunteers/Index");
                }
               
            }
            catch
            {
                return View();
            }
        }
        public ActionResult UpdateProfile()
        {
            
            var volunteer = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(Session["UserId"].ToString()));
            return View(volunteer);
        }
        [HttpPost]
            public ActionResult UpdateProfile(HttpPostedFileBase file,  VolunteerModel volunteer)
        {

            if (file != null && file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(Server.MapPath("/UserImages/"), fileName);
                file.SaveAs(filePath);

                var filter = Builders<VolunteerModel>.Filter.Eq("_id", ObjectId.Parse(Session["UserId"].ToString()));
                var update = Builders<VolunteerModel>.Update
                    .Set("UserPhoto", "/UserImages/" + file.FileName);
                var result = volunteerCollection.UpdateOne(filter, update);
                var volunteerName = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Id == ObjectId.Parse(Session["UserId"].ToString()));
                Session["Img"] = volunteerName.UserPhoto.ToString();

            }
            return View("../Home/Index");
        }

        public ActionResult AddRole(string id)
        {
            var list = new List<string>() { null, "Admin", "Moderator", "Volunteer" };
            ViewBag.list = list;
            var volunteerId = new ObjectId(id);
            var volunteer = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Id == volunteerId);
           
            return View(volunteer);
        }
        
       
        [HttpPost]
        public ActionResult AddRole(string id, VolunteerModel volunteer)
        {
            var list = new List<string>() { null, "Admin", "Moderator", "Volunteer" };
            ViewBag.list = list;
            var volunteerId = new ObjectId(id);
            var vol = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Id == volunteerId);
            var email = vol.Email;

            try
            {
                // TODO: Add update logic here
                var filter = Builders<VolunteerModel>.Filter.Eq("_id", ObjectId.Parse(id));
                if (volunteer.Role == null)
                {
                    var update = Builders<VolunteerModel>.Update
                   .Set("Active", "No")
                   .Set("Role", volunteer.Role);

                    var result = volunteerCollection.UpdateOne(filter, update);
                }
                else
                {
                    

                    MailAddressModel mode = new MailAddressModel();
                    mode.To = email;
                    mode.From = Session["Email"].ToString();
                    mode.Subject = "Hello";
                    mode.Body = "Hello, your account has been activated!!! Please log in ...";
                    SendEmail(mode);
                    var update = Builders<VolunteerModel>.Update
                   .Set("Active", "Yes")
                   .Set("Role", volunteer.Role);
                    var result = volunteerCollection.UpdateOne(filter, update);

                }

                List<VolunteerModel> volunteers = volunteerCollection.AsQueryable<VolunteerModel>().ToList();

                volunteerList = new List<string>();

                foreach (var volun in volunteers)
                {
                    if (volun.Active == "Yes")
                    {
                        volunteerList.Add(volun.Name.ToString());

                    }

                }
                Session["VolunteerList"] = volunteerList;
                return RedirectToAction("../Volunteers/Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult EditRole(string id)
        {

            var list = new List<string>() { null, "Admin", "Moderator", "Volunteer" };
            ViewBag.list = list;
            var volunteerId = new ObjectId(id);
            var volunteer = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Id == volunteerId);
            return View(volunteer);
        }

        [HttpPost]
        public ActionResult EditRole(string id, VolunteerModel volunteer)
        {
            var list = new List<string>() { null, "Admin", "Moderator", "Volunteer" };
            ViewBag.list = list;
            var volunteerId = new ObjectId(id);
            var vol = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Id == volunteerId);
            var email = vol.Email;
            
            try
            {
                // TODO: Add update logic here
                var filter = Builders<VolunteerModel>.Filter.Eq("_id", ObjectId.Parse(id));
                if (volunteer.Role==null)
                {
                    var update = Builders<VolunteerModel>.Update
                   .Set("Active", "No")
                   .Set("Role", volunteer.Role);

                    var result = volunteerCollection.UpdateOne(filter, update);
                }
                else
                {
                    
                    MailAddressModel mode = new MailAddressModel();
                    mode.To = email;
                    mode.From = Session["Email"].ToString();
                    mode.Subject = "Hello";
                    mode.Body = "Hello, your account has been activated!!! Please log in ...";
                    SendEmail(mode);
                    var update = Builders<VolunteerModel>.Update
                   .Set("Active", "Yes")
                   .Set("Role", volunteer.Role);

                    var result = volunteerCollection.UpdateOne(filter, update);
                }
                List<VolunteerModel> volunteers = volunteerCollection.AsQueryable<VolunteerModel>().ToList();

                volunteerList = new List<string>();

                foreach (var volun in volunteers)
                {
                    if (volun.Active == "Yes")
                    {
                        volunteerList.Add(volun.Name.ToString());

                    }

                }
                Session["VolunteerList"] = volunteerList;
                return RedirectToAction("../Volunteers/Index");
            }
            catch
            {
                return View();
            }
        }
        public void SendEmail (MailAddressModel model) {
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
