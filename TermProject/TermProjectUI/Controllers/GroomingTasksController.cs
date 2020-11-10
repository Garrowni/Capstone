
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

    public class GroomingTasksController : Controller
    {
        // private InventoryTaskDBModelContainer db = new TransportTaskDBModelContainer();

        //private InventoryTaskRepoEF ttr = new InventoryTaskRepoEF();

        static List<GroomingTaskModel.services> servicesList = new List<GroomingTaskModel.services>();
        static List<string> assignees = new List<string>();


        static List<Object> deletedTask = new List<Object>();

        private MongoDBContext dbcontext;
        private IMongoCollection<GroomingTaskModel> productCollection;
        private IMongoCollection<DeletedTaskModel> deletedCollection;
        private IMongoCollection<VolunteerModel> volunteerCollection;

        public GroomingTasksController()
        {
            dbcontext = new MongoDBContext();
            productCollection = dbcontext.database.GetCollection<GroomingTaskModel>("grooming");
            deletedCollection = dbcontext.database.GetCollection<DeletedTaskModel>("deletedTasks");
            volunteerCollection = dbcontext.database.GetCollection<VolunteerModel>("volunteer");

        }
        // GET: TransportationTasks
        public ActionResult Index()
        {
            // return View(db.TransportationTasks.ToList());
            //return View(ttr.GetTransportationTasks());

            List<GroomingTaskModel> tasks = productCollection.AsQueryable<GroomingTaskModel>().ToList();
            return View(tasks);
        }

        // GET: TransportationTasks/Details/5
        public ActionResult Details(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<GroomingTaskModel>().SingleOrDefault(x => x.Id == taskId);
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
        public ActionResult Create(GroomingTaskModel groomingTask)
        {
            groomingTask.posterName = Session["Username"].ToString();
            groomingTask.posterPhoto = Session["Img"].ToString();
            groomingTask.taskType = "Grooming Task";
            groomingTask.taskName = "GroomingTaskTest";
          
            groomingTask.state = "Unassigned";

            groomingTask.Services = servicesList;
  

            try
            {
                productCollection.InsertOne(groomingTask);
                servicesList = new List<GroomingTaskModel.services>();
                deletedTask = new List<object>();
                deletedTask.Add(groomingTask);
                
             
                return RedirectToAction("Details", new { id = groomingTask.Id });
                
            }
            catch
            {
                return View();
            }
        }

        public JsonResult InsertDocuments(List<GroomingTaskModel.services> itemsSpec)
        {

            //Check for NULL.
            if (itemsSpec == null)
            {
                itemsSpec = new List<GroomingTaskModel.services>();
            }



            foreach (GroomingTaskModel.services itemSpec in itemsSpec)
            {

                servicesList.Add(itemSpec);

            }
            //Debug.WriteLine(taskSpecList[0].Key);
            int insertedRecords = servicesList.Count();
            return Json(insertedRecords);

        }
        public JsonResult UpdateDocuments(List<GroomingTaskModel.services> tasksSpec)
        {

            //Check for NULL.
            if (tasksSpec == null)
            {
                tasksSpec = new List<GroomingTaskModel.services>();
            }

            servicesList = new List<GroomingTaskModel.services>();
            foreach (GroomingTaskModel.services taskSpec in tasksSpec)
            {
                servicesList.Add(taskSpec);

            }

            int insertedRecords = servicesList.Count();
            return Json(insertedRecords);

        }



        // GET: TransportationTasks/Edit/5
        public ActionResult Edit(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<GroomingTaskModel>().SingleOrDefault(x => x.Id == taskId);
            return View(task);
        }

        // POST: TransportationTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, GroomingTaskModel task)
        {
            
            try
            {
                var filter = Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<GroomingTaskModel>.Update
                   
                    .Set("AdditionalInfo", task.AdditionalInfo)
                    .Set("taskID", task.taskID)
                    .Set("taskName", task.taskName)
                    .Set("taskType", task.taskType)
                    .Set("posterName", task.posterName)
                    .Set("creationDate", task.creationDate)
                    .Set("state", task.state)
                    .Set("dogName", task.dogName)
                    .Set("dogAge", task.dogAge)
                    .Set("dogBreed", task.dogBreed)
                    .Set("dogSize", task.dogSize)
                    .Set("Services", task.Services)
                    .Set("booked", task.booked)
                    .Set("bookedAddress", task.bookedAddress)
                    .Set("bookedStore", task.bookedStore)
                    .Set("bookedDate", task.bookedDate)
                    .Set("bookedTime", task.bookedTime)
                    .Set("prefStore", task.prefStore)
                    .Set("prefAddress", task.prefAddress)
                    .Set("prefDate", task.prefDate)
                    .Set("paid", task.paid)
                    .Set("price", task.price)
                    .Set("payer", task.payer)
                    .Set("receipt", task.receipt);
                var result = productCollection.UpdateOne(filter, update);
                deletedTask = new List<object>();
                task.Id = ObjectId.Parse(id);
                deletedTask.Add(task);

                servicesList = new List<GroomingTaskModel.services>();

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
            var task = productCollection.AsQueryable<GroomingTaskModel>().SingleOrDefault(x => x.Id == taskId);
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
                taskDelete.tasksType = "Grooming";
                deletedCollection.InsertOne(taskDelete);
                deletedTask = new List<Object>();
                productCollection.DeleteOne(Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id)));
                
                return RedirectToAction("../AllTasks/Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult JoinTask(string id, GroomingTaskModel task)
        {
            assignees.Add(Session["UserId"].ToString());
            task.assignees = assignees;

            var filter = Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<GroomingTaskModel>.Update
                .Set("assignees", assignees)
                .Set("state", "Assigned");
            var result = productCollection.UpdateOne(filter, update);

            assignees = new List<string>();
            return RedirectToAction("Details", new { id = id });



        }
        public ActionResult DisjointTask(string id, GroomingTaskModel task)
        {
            assignees.Remove(Session["UserId"].ToString());
            if (assignees.Count == 0 || assignees == null)
            {
                task.assignees = assignees;

                var filter = Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<GroomingTaskModel>.Update
                    .Set("assignees", assignees)
                     .Set("state", "Unassigned");
                var result = productCollection.UpdateOne(filter, update);

                assignees = new List<string>();
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                task.assignees = assignees;

                var filter = Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<GroomingTaskModel>.Update
                    .Set("assignees", assignees);
                var result = productCollection.UpdateOne(filter, update);

                assignees = new List<string>();
                return RedirectToAction("Details", new { id = id });
            }



        }


    }
}
