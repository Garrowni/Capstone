
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

    public class InventoryTasksController : Controller
    {
        // private InventoryTaskDBModelContainer db = new TransportTaskDBModelContainer();

        //private InventoryTaskRepoEF ttr = new InventoryTaskRepoEF();
        static List<InventoryTaskModel.documents> documentsList = new List<InventoryTaskModel.documents>();



        static List<Object> deletedTask = new List<Object>();

        private MongoDBContext dbcontext;
        private IMongoCollection<InventoryTaskModel> productCollection;
        private IMongoCollection<DeletedTaskModel> deletedCollection;

        public InventoryTasksController()
        {
            dbcontext = new MongoDBContext();
            productCollection = dbcontext.database.GetCollection<InventoryTaskModel>("inventory");
            deletedCollection = dbcontext.database.GetCollection<DeletedTaskModel>("deletedTasks");

        }
        // GET: TransportationTasks
        public ActionResult Index()
        {


            List<InventoryTaskModel> tasks = productCollection.AsQueryable<InventoryTaskModel>().ToList();
            return View(tasks);
        }

        // GET: TransportationTasks/Details/5
        public ActionResult Details(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<InventoryTaskModel>().SingleOrDefault(x => x.Id == taskId);
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
        public ActionResult Create(InventoryTaskModel inventoryTask)
        {
            inventoryTask.posterName = "Nicole Garrow";
            inventoryTask.taskType = "Inventory Task";
            inventoryTask.taskName = "InventoryTaskTest";
            inventoryTask.requester = "Ellie";
          
            inventoryTask.state = "Unassigned";

            inventoryTask.Documents = documentsList;
  

            try
            {
                productCollection.InsertOne(inventoryTask);
                documentsList = new List<InventoryTaskModel.documents>();
                deletedTask = new List<object>();
                deletedTask.Add(inventoryTask);
                
             
                return RedirectToAction("Details", new { id = inventoryTask.Id });
                
            }
            catch
            {
                return View();
            }
        }

        public JsonResult InsertDocuments(List<InventoryTaskModel.documents> itemsSpec)
        {

            //Check for NULL.
            if (itemsSpec == null)
            {
                itemsSpec = new List<InventoryTaskModel.documents>();
            }

   

            foreach (InventoryTaskModel.documents itemSpec in itemsSpec)
            {

                documentsList.Add(itemSpec);

            }
            //Debug.WriteLine(taskSpecList[0].Key);
            int insertedRecords = documentsList.Count();
            return Json(insertedRecords);

        }
        public JsonResult UpdateDocuments(List<InventoryTaskModel.documents> tasksSpec)
        {

            //Check for NULL.
            if (tasksSpec == null)
            {
                tasksSpec = new List<InventoryTaskModel.documents>();
            }

            documentsList = new List<InventoryTaskModel.documents>();
            foreach (InventoryTaskModel.documents taskSpec in tasksSpec)
            {
                documentsList.Add(taskSpec);

            }

            int insertedRecords = documentsList.Count();
            return Json(insertedRecords);

        }


        // GET: TransportationTasks/Edit/5
        public ActionResult Edit(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<InventoryTaskModel>().SingleOrDefault(x => x.Id == taskId);
            return View(task);
        }

        // POST: TransportationTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, InventoryTaskModel task)
        {
            task.Documents = documentsList;
            try
            {
                var filter = Builders<InventoryTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<InventoryTaskModel>.Update


                    .Set("taskID", task.taskID)
                    .Set("taskName", task.taskName)
                    .Set("taskType", task.taskType)
                    .Set("posterName", task.posterName)
                    .Set("creationDate", task.creationDate)
                    .Set("state", task.state)
                    .Set("address", task.address)
                    .Set("date", task.date)
                    .Set("time", task.time)
                    .Set("AdditionalInfo", task.AdditionalInfo)
                    .Set("Documents", task.Documents);                    ;
                var result = productCollection.UpdateOne(filter, update);
                deletedTask = new List<object>();
                task.Id = ObjectId.Parse(id);
                deletedTask.Add(task);

                documentsList = new List<InventoryTaskModel.documents>();

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
            var task = productCollection.AsQueryable<InventoryTaskModel>().SingleOrDefault(x => x.Id == taskId);
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
                productCollection.DeleteOne(Builders<InventoryTaskModel>.Filter.Eq("_id", ObjectId.Parse(id)));
                
                return RedirectToAction("../AllTasks/Index");
            }
            catch
            {
                return View();
            }
        }

       
    }
}
