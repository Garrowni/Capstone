
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

         static List<TransportationTaskModel.Item> x = new List<TransportationTaskModel.Item>();
      


        private MongoDBContext dbcontext;
        private IMongoCollection<TransportationTaskModel> productCollection;
       
        public TransportationTasksController()
        {
            dbcontext = new MongoDBContext();
            productCollection = dbcontext.database.GetCollection<TransportationTaskModel>("product");

        }
        // GET: TransportationTasks
        public ActionResult Index()
        {
            // return View(db.TransportationTasks.ToList());
            //return View(ttr.GetTransportationTasks());

            List<TransportationTaskModel> products = productCollection.AsQueryable<TransportationTaskModel>().ToList();
            return View(products);
        }

        // GET: TransportationTasks/Details/5
        public ActionResult Details(string id)
        {
            var productId = new ObjectId(id);
            var product = productCollection.AsQueryable<TransportationTaskModel>().SingleOrDefault(x => x.Id == productId);
            return View(product);
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
            
            transportationTask.Items = x;
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
                x = new List<TransportationTaskModel.Item>();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
       
        public JsonResult InsertItems(List<TransportationTaskModel.Item> customers)
        {
           
                //Check for NULL.
                if (customers == null)
                {
                    customers = new List<TransportationTaskModel.Item>();
                }

                
                foreach (TransportationTaskModel.Item customer in customers)
                {
                    
                    x.Add(customer);

                }
                Debug.WriteLine(x[0].ItemName);
                int insertedRecords = x.Count();
                return Json(insertedRecords);
            
        }

       
        public JsonResult UpdateItems(List<TransportationTaskModel.Item> customers)
        {
            
            //Check for NULL.
            if (customers == null)
            {
                customers = new List<TransportationTaskModel.Item>();
            }

            x= new List<TransportationTaskModel.Item>();
            foreach (TransportationTaskModel.Item customer in customers)
            {
                x.Add(customer);

            }
            Debug.WriteLine(x[0].ItemName);
            int insertedRecords = x.Count();
            return Json(insertedRecords);

        }
        // GET: TransportationTasks/Edit/5
        public ActionResult Edit(string id)
        {
            var productId = new ObjectId(id);
            var product = productCollection.AsQueryable<TransportationTaskModel>().SingleOrDefault(x => x.Id == productId);
            return View(product);
        }

        // POST: TransportationTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, TransportationTaskModel product)
        {
            product.Items = x;
            try
            {
                var filter = Builders<TransportationTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<TransportationTaskModel>.Update
                    .Set("ImportanceLevel", product.ImportanceLevel)
                    .Set("Requester", product.Requester)
                    .Set("PUAddress", product.PUAddress)
                    .Set("PUCity", product.PUCity)
                    .Set("PUPostal", product.PUPostal)
                    .Set("PUName", product.PUName)
                    .Set("PUDate", product.PUDate)
                    .Set("PUTime", product.PUTime)
                    .Set("PUAdditional", product.PUAdditional)
                    .Set("DOAddress", product.DOAddress)
                    .Set("DOCity", product.DOCity)
                    .Set("DOPostal", product.DOPostal)
                    .Set("DOName", product.DOName)
                    .Set("DODate", product.DODate)
                    .Set("DOTime", product.DOTime)
                    .Set("DOAdditional", product.DOAdditional)
                    .Set("AdditionalInfo", product.AdditionalInfo)
                    .Set("Items",product.Items)
                    .Set("taskID", product.taskID)
                    .Set("taskName", product.taskName)
                    .Set("taskType", product.taskType)
                    .Set("posterName", product.posterName)
                    .Set("creationDate", product.creationDate)
                    .Set("state", product.state);
                var result = productCollection.UpdateOne(filter, update);
                
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TransportationTasks/Delete/5
        public ActionResult Delete(string id)
        {
            var productId = new ObjectId(id);
            var product = productCollection.AsQueryable<TransportationTaskModel>().SingleOrDefault(x => x.Id == productId);
            return View(product);
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
