
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
//using DBModel;
using TermProjectUI.Models;
using TermProjectUI.App_Start;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics;

namespace TermProjectUI.Controllers
{

    public class PhotographyTasksController : Controller
    {

        static List<PhotographyTaskModel.dog> dogsList = new List<PhotographyTaskModel.dog>();
        static List<string> assignees = new List<string>();


        static List<Object> deletedTask = new List<Object>();

        private MongoDBContext dbcontext;
        private IMongoCollection<PhotographyTaskModel> productCollection;
        private IMongoCollection<DeletedTaskModel> deletedCollection;
        private IMongoCollection<VolunteerModel> volunteerCollection;
        public PhotographyTasksController()
        {
            dbcontext = new MongoDBContext();
            productCollection = dbcontext.database.GetCollection<PhotographyTaskModel>("photography");
            deletedCollection = dbcontext.database.GetCollection<DeletedTaskModel>("deletedTasks");
            volunteerCollection = dbcontext.database.GetCollection<VolunteerModel>("volunteer");
        }
        // GET: TransportationTasks
        public ActionResult Index()
        {
            // return View(db.TransportationTasks.ToList());
            //return View(ttr.GetTransportationTasks());

            List<PhotographyTaskModel> tasks = productCollection.AsQueryable<PhotographyTaskModel>().ToList();
            return View(tasks);
        }

        // GET: TransportationTasks/Details/5
        public ActionResult Details(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<PhotographyTaskModel>().SingleOrDefault(x => x.Id == taskId);
            ViewBag.req = task.requester;
            ViewBag.post = task.posterName;
            ViewBag.state = task.state;
            assignees = new List<string>();
            bool assignedForTask = false;
            if (task.assignees != null)
            {
                List<string> assigneeNames = new List<string>();

                foreach (var assignee in task.assignees)
                {
                    assignees.Add(assignee);
                    if (assignee == Session["UserId"].ToString())
                    {
                        assignedForTask = true;
                    }
                    var volunteerId = new ObjectId(assignee);
                    var volunteer = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Id == volunteerId);
                    assigneeNames.Add(volunteer.Name);
                }
                ViewBag.Message = assignedForTask;
                ViewBag.AssigneeNames = assigneeNames;
            }
            else
            {
                assignees = new List<string>();
                assignedForTask = false;
                ViewBag.Message = assignedForTask;

            }
            return View(task);
        }

        // GET: TransportationTasks/Create
        public ActionResult Create()
        {
            return View();
        }

      
            // POST: TransportationTasks/Create
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PhotographyTaskModel photographyTask)
        {
            photographyTask.posterName = Session["Username"].ToString();
            photographyTask.posterPhoto = Session["Img"].ToString();
            photographyTask.taskType = "Photography Task";
            photographyTask.taskName = photographyTask.taskType + " - " + photographyTask.photographerName;

            photographyTask.state = "Unassigned";
            var vol = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Name == photographyTask.requester);
            photographyTask.reqPhoto = vol.UserPhoto;
            photographyTask.Dogs = dogsList;

            try
            {
                productCollection.InsertOne(photographyTask);
                
                deletedTask = new List<object>();
                deletedTask.Add(photographyTask);

                dogsList = new List<PhotographyTaskModel.dog>();
                return RedirectToAction("Details", new { id = photographyTask.Id });
                
            }
            catch
            {
                return View();
            }
        }


        public JsonResult InsertDocuments(List<PhotographyTaskModel.dog> items)
        {

            //Check for NULL.
            if (items == null)
            {
                items = new List<PhotographyTaskModel.dog>();
            }


            foreach (PhotographyTaskModel.dog item in items)
            {

                dogsList.Add(item);

            }
            //Debug.WriteLine(itemList[0].ItemName);
            int insertedRecords = dogsList.Count();

            return Json(insertedRecords);

        }


        public JsonResult UpdateItems(List<PhotographyTaskModel.dog> items)
        {

            //Check for NULL.
            if (items == null)
            {
                items = new List<PhotographyTaskModel.dog>();
            }

            dogsList = new List<PhotographyTaskModel.dog>();
            foreach (PhotographyTaskModel.dog item in items)
            {
                dogsList.Add(item);

            }
            // Debug.WriteLine(itemList[0].ItemName);
            int insertedRecords = dogsList.Count();
            return Json(insertedRecords);


        }


        // GET: TransportationTasks/Edit/5
        public ActionResult Edit(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<PhotographyTaskModel>().SingleOrDefault(x => x.Id == taskId);
            return View(task);
        }

        // POST: TransportationTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, PhotographyTaskModel task)
        {
            task.Dogs = dogsList;
            
            try
            {
                var filter = Builders<PhotographyTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<PhotographyTaskModel>.Update
                   
                    .Set("AdditionalInfo", task.AdditionalInfo)
                    .Set("taskID", task.taskID)
                    .Set("taskName", task.taskName)
                    .Set("taskType", task.taskType)
                    .Set("posterName", task.posterName)
                    .Set("creationDate", task.creationDate)
                    .Set("state", task.state)
                    .Set("location", task.location)
                    .Set("photographerName", task.photographerName)
                    .Set("taskDate", task.taskDate)
                    .Set("taskTime", task.taskTime)
                    .Set("Dogs", task.Dogs)
                    .Set("AdditionalInfo", task.AdditionalInfo);
                var result = productCollection.UpdateOne(filter, update);
                deletedTask = new List<object>();
                task.Id = ObjectId.Parse(id);
                deletedTask.Add(task);

                dogsList = new List<PhotographyTaskModel.dog>();

                return RedirectToAction("Details", new { id = id });
            }
            catch
            {
                return View();
            }
        }

        // GET: TransportationTasks/Delete/5
        public ActionResult Delete(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<PhotographyTaskModel>().SingleOrDefault(x => x.Id == taskId);
            return View(task);
        }

        // POST: TransportationTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, DeletedTaskModel taskDelete)
        {
            //  TransportationTask transportationTask = db.TransportationTasks.Find(id);
            //  db.TransportationTasks.Remove(transportationTask);
            //  db.SaveChanges();

            try
            {

                
                taskDelete.deletedTask = deletedTask;
                taskDelete.tasksType = "Inventory";
                deletedCollection.InsertOne(taskDelete);
                deletedTask = new List<Object>();
                productCollection.DeleteOne(Builders<PhotographyTaskModel>.Filter.Eq("_id", ObjectId.Parse(id)));
                
                return RedirectToAction("../AllTasks/Index");
            }
            catch
            {
                return View();
            }

        }
        public ActionResult JoinTask(string id, PhotographyTaskModel task)
        {
            assignees.Add(Session["UserId"].ToString());
            task.assignees = assignees;

            var filter = Builders<PhotographyTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<PhotographyTaskModel>.Update
                .Set("assignees", assignees)
                .Set("state", "Assigned");
            var result = productCollection.UpdateOne(filter, update);

            assignees = new List<string>();
            return RedirectToAction("Details", new { id = id });



        }
        public ActionResult DisjointTask(string id, PhotographyTaskModel task)
        {
            assignees.Remove(Session["UserId"].ToString());
            if (assignees.Count == 0 || assignees == null)
            {
                task.assignees = assignees;

                var filter = Builders<PhotographyTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<PhotographyTaskModel>.Update
                    .Set("assignees", assignees)
                     .Set("state", "Unassigned");
                var result = productCollection.UpdateOne(filter, update);

                assignees = new List<string>();
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                task.assignees = assignees;

                var filter = Builders<PhotographyTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<PhotographyTaskModel>.Update
                    .Set("assignees", assignees);
                var result = productCollection.UpdateOne(filter, update);

                assignees = new List<string>();
                return RedirectToAction("Details", new { id = id });
            }

        }

        public ActionResult CompleteTask(string id, PhotographyTaskModel task)
        {


            var filter = Builders<PhotographyTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<PhotographyTaskModel>.Update
                 .Set("state", "Completed");
            var result = productCollection.UpdateOne(filter, update);
            if (Session["Role"].ToString() == "Admin" || Session["Role"].ToString() == "Moderator")
            {
                return RedirectToAction("../CompletedTasks/Index");
            }
            else
            {
                return RedirectToAction("../AllTasks/Index");
            }
        }


    }
}
