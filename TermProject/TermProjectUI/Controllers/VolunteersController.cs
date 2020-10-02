using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public VolunteersController()
        {
            dbcontext = new MongoDBContext();
            volunteerCollection = dbcontext.database.GetCollection<VolunteerModel>("volunteer");

        }
        // GET: Members
        public ActionResult Index()
        {
            List<VolunteerModel> volunteer = volunteerCollection.AsQueryable<VolunteerModel>().ToList();
            return View(volunteer);
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
            try
            {
                // TODO: Add insert logic here
                volunteerCollection.InsertOne(volunteer);
                return RedirectToAction("Details", new { id = volunteer.Id });
            }
            catch
            {
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
                    .Set("Email", volunteer.Email)
                    .Set("Role", volunteer.Role);

                var result = volunteerCollection.UpdateOne(filter, update);
                return RedirectToAction("Index", new { id = id });
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

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
