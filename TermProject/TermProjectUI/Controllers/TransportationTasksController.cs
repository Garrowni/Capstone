
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

    public class TransportationTasksController : Controller
    {
        // private TransportTaskDBModelContainer db = new TransportTaskDBModelContainer();

        //private TransportationTaskRepoEF ttr = new TransportationTaskRepoEF();

         static List<TransportationTaskModel.Item> itemList = new List<TransportationTaskModel.Item>();
      


        private MongoDBContext dbcontext;
        private IMongoCollection<TransportationTaskModel> productCollection;
       
        public TransportationTasksController()
        {
            dbcontext = new MongoDBContext();
            productCollection = dbcontext.database.GetCollection<TransportationTaskModel>("transportation");

        }
        // GET: TransportationTasks
        public ActionResult Index()
        {
            // return View(db.TransportationTasks.ToList());
            //return View(ttr.GetTransportationTasks());

            List<TransportationTaskModel> tasks = productCollection.AsQueryable<TransportationTaskModel>().ToList();
            return View(tasks);
        }

        // GET: TransportationTasks/Details/5
        public ActionResult Details(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<TransportationTaskModel>().SingleOrDefault(x => x.Id == taskId);
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
        public ActionResult Create(TransportationTaskModel transportationTask)
        {
            transportationTask.posterName = "Nicole Garrow";
            transportationTask.taskType = "Transportation Task";
            transportationTask.taskName = transportationTask.taskType + " - " + transportationTask.PUCity + " to " + transportationTask.DOCity;
            
            transportationTask.Items = itemList;
            transportationTask.creationDate = DateTime.Today;
            //transportationTask.assignees = "0";
            transportationTask.state = "Unassigned";
           /** if (ModelState.IsValid)
            {
                //  db.TransportationTasks.Add(transportationTask);
                //db.SaveChanges();

                ttr.Add(transportationTask);
              //  return RedirectToAction("Index");
                return RedirectToAction("Details", new { id = transportationTask.taskID });
            }

            return View(transportationTask);*/


            try
            {
                productCollection.InsertOne(transportationTask);
                itemList = new List<TransportationTaskModel.Item>();

                return RedirectToAction("Details", new { id = transportationTask.Id });
                
            }
            catch
            {
                return View();
            }
        }
       
        public JsonResult InsertItems(List<TransportationTaskModel.Item> items)
        {
           
                //Check for NULL.
                if (items == null)
                {
                    items = new List<TransportationTaskModel.Item>();
                }

                
                foreach (TransportationTaskModel.Item item in items)
                {

                itemList.Add(item);

                }
                //Debug.WriteLine(itemList[0].ItemName);
                int insertedRecords = itemList.Count();
                return Json(insertedRecords);
            
        }

       
        public JsonResult UpdateItems(List<TransportationTaskModel.Item> items)
        {
            
            //Check for NULL.
            if (items == null)
            {
                items = new List<TransportationTaskModel.Item>();
            }

            itemList = new List<TransportationTaskModel.Item>();
            foreach (TransportationTaskModel.Item item in items)
            {
                itemList.Add(item);

            }
           // Debug.WriteLine(itemList[0].ItemName);
            int insertedRecords = itemList.Count();
            return Json(insertedRecords);

        }
        // GET: TransportationTasks/Edit/5
        public ActionResult Edit(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<TransportationTaskModel>().SingleOrDefault(x => x.Id == taskId);
            return View(task);
        }

        // POST: TransportationTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, TransportationTaskModel task)
        {
            task.Items = itemList;
            try
            {
                var filter = Builders<TransportationTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<TransportationTaskModel>.Update
                    .Set("ImportanceLevel", task.ImportanceLevel)
                    .Set("Requester", task.Requester)
                    .Set("PUAddress", task.PUAddress)
                    .Set("PUCity", task.PUCity)
                    .Set("PUPostal", task.PUPostal)
                    .Set("PUName", task.PUName)
                    .Set("PUDate", task.PUDate)
                    .Set("PUTime", task.PUTime)
                    .Set("PUAdditional", task.PUAdditional)
                    .Set("DOAddress", task.DOAddress)
                    .Set("DOCity", task.DOCity)
                    .Set("DOPostal", task.DOPostal)
                    .Set("DOName", task.DOName)
                    .Set("DODate", task.DODate)
                    .Set("DOTime", task.DOTime)
                    .Set("DOAdditional", task.DOAdditional)
                    .Set("AdditionalInfo", task.AdditionalInfo)
                    .Set("Items", task.Items)
                    .Set("taskID", task.taskID)
                    .Set("taskName", task.taskName)
                    .Set("taskType", task.taskType)
                    .Set("posterName", task.posterName)
                    .Set("creationDate", task.creationDate)
                    .Set("state", task.state);
                var result = productCollection.UpdateOne(filter, update);
                itemList = new List<TransportationTaskModel.Item>();
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
            var task = productCollection.AsQueryable<TransportationTaskModel>().SingleOrDefault(x => x.Id == taskId);
            return View(task);
        }

        // POST: TransportationTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            //  TransportationTask transportationTask = db.TransportationTasks.Find(id);
            //  db.TransportationTasks.Remove(transportationTask);
            //  db.SaveChanges();

            try
            {
                productCollection.DeleteOne(Builders<TransportationTaskModel>.Filter.Eq("_id", ObjectId.Parse(id)));

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

       
    }
}
