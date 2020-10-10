
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
      
       

        static List<Object> deletedTask = new List<Object>();

        private MongoDBContext dbcontext;
        private IMongoCollection<PhotographyTaskModel> productCollection;
        private IMongoCollection<DeletedTaskModel> deletedCollection;

        public PhotographyTasksController()
        {
            dbcontext = new MongoDBContext();
            productCollection = dbcontext.database.GetCollection<PhotographyTaskModel>("photography");
            deletedCollection = dbcontext.database.GetCollection<DeletedTaskModel>("deletedTasks");

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

            photographyTask.Dogs = dogsList;

            try
            {
                productCollection.InsertOne(photographyTask);
                dogsList = new List<PhotographyTaskModel.dog>();
                deletedTask = new List<object>();
                deletedTask.Add(photographyTask);
                
             
                return RedirectToAction("Details", new { id = photographyTask.Id });
                
            }
            catch
            {
                return View();
            }
        }


        public JsonResult InsertDocuments(List<PhotographyTaskModel.dog> itemsSpec)
        {

            //Check for NULL.
            if (itemsSpec == null)
            {
                itemsSpec = new List<PhotographyTaskModel.dog>();
            }



            foreach (PhotographyTaskModel.dog itemSpec in itemsSpec)
            {

                dogsList.Add(itemSpec);

            }
            //Debug.WriteLine(taskSpecList[0].Key);
            int insertedRecords = dogsList.Count();
            return Json(insertedRecords);

        }
        public JsonResult UpdateDocuments(List<PhotographyTaskModel.dog> tasksSpec)
        {

            //Check for NULL.
            if (tasksSpec == null)
            {
                tasksSpec = new List<PhotographyTaskModel.dog>();
            }

            dogsList = new List<PhotographyTaskModel.dog>();
            foreach (PhotographyTaskModel.dog taskSpec in tasksSpec)
            {
                dogsList.Add(taskSpec);

            }

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

       
    }
}
