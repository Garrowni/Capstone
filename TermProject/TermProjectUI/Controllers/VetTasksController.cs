﻿
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

    public class VetTasksController : Controller
    {
        static List<VetTaskModel.documents> documentsList = new List<VetTaskModel.documents>();



        static List<Object> deletedTask = new List<Object>();

        private MongoDBContext dbcontext;
        private IMongoCollection<VetTaskModel> productCollection;
        private IMongoCollection<DeletedTaskModel> deletedCollection;

        public VetTasksController()
        {
            dbcontext = new MongoDBContext();
            productCollection = dbcontext.database.GetCollection<VetTaskModel>("vet");
            deletedCollection = dbcontext.database.GetCollection<DeletedTaskModel>("deletedTasks");

        }
        // GET: TransportationTasks
        public ActionResult Index()
        {
            // return View(db.TransportationTasks.ToList());
            //return View(ttr.GetTransportationTasks());

            List<VetTaskModel> tasks = productCollection.AsQueryable<VetTaskModel>().ToList();
            return View(tasks);
        }

        // GET: TransportationTasks/Details/5
        public ActionResult Details(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<VetTaskModel>().SingleOrDefault(x => x.Id == taskId);
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
        public ActionResult Create(VetTaskModel vetTask)
        {
            vetTask.posterName = Session["Username"].ToString();
            vetTask.posterPhoto = Session["Img"].ToString();
            vetTask.taskType = "Vet Task";
            vetTask.taskName = "VetTaskTest";
          
            vetTask.state = "Unassigned";

            vetTask.Documents = documentsList;

            try
            {
                productCollection.InsertOne(vetTask);
                documentsList = new List<VetTaskModel.documents>();
                deletedTask = new List<object>();
                deletedTask.Add(vetTask);
                
             
                return RedirectToAction("Details", new { id = vetTask.Id });
                
            }
            catch
            {
                return View();
            }
        }

        public JsonResult InsertDocuments(List<VetTaskModel.documents> itemsSpec)
        {

            //Check for NULL.
            if (itemsSpec == null)
            {
                itemsSpec = new List<VetTaskModel.documents>();
            }



            foreach (VetTaskModel.documents itemSpec in itemsSpec)
            {

                documentsList.Add(itemSpec);

            }
            //Debug.WriteLine(taskSpecList[0].Key);
            int insertedRecords = documentsList.Count();
            return Json(insertedRecords);

        }
        public JsonResult UpdateDocuments(List<VetTaskModel.documents> tasksSpec)
        {

            //Check for NULL.
            if (tasksSpec == null)
            {
                tasksSpec = new List<VetTaskModel.documents>();
            }

            documentsList = new List<VetTaskModel.documents>();
            foreach (VetTaskModel.documents taskSpec in tasksSpec)
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
            var task = productCollection.AsQueryable<VetTaskModel>().SingleOrDefault(x => x.Id == taskId);
            return View(task);
        }

        // POST: TransportationTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, VetTaskModel task)
        {
            
            try
            {
                var filter = Builders<VetTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<VetTaskModel>.Update
                   
                    
                    .Set("taskID", task.taskID)
                    .Set("taskName", task.taskName)
                    .Set("taskType", task.taskType)
                    .Set("posterName", task.posterName)
                    .Set("creationDate", task.creationDate)
                    .Set("state", task.state)
                    .Set("AdditionalInfo", task.AdditionalInfo)
                    .Set("pickupLocation", task.pickupLocation)
                    .Set("pickupVolunteer", task.pickupVolunteer)
                    .Set("pickupDate", task.pickupDate)
                    .Set("pickupTime", task.pickupTime)
                    .Set("appointmentAddress", task.appointmentAddress)
                    .Set("appointmentDate", task.appointmentDate)
                    .Set("appointmentTime", task.appointmentTime)
                    .Set("vet", task.vetName)
                    .Set("appointmentReason", task.appointmentReason)
                    .Set("appointmentNotes", task.appointmentNotes)
                    .Set("dropoffLocation", task.dropoffLocation)
                    .Set("dropoffVolunteer", task.dropoffVolunteer)
                    .Set("dropoffDate", task.dropoffDate)
                    .Set("dropoffTime", task.dropoffTime)
                    .Set("dogName", task.dogName)
                    .Set("dogBreed", task.dogBreed)
                    .Set("dogSize", task.dogSize)
                    .Set("dogNotes", task.dogNotes)
                    .Set("Documents", task.Documents);

                var result = productCollection.UpdateOne(filter, update);
                deletedTask = new List<object>();
                task.Id = ObjectId.Parse(id);
                deletedTask.Add(task);

                documentsList = new List<VetTaskModel.documents>();

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
            var task = productCollection.AsQueryable<VetTaskModel>().SingleOrDefault(x => x.Id == taskId);
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
                taskDelete.tasksType = "Vet";
                deletedCollection.InsertOne(taskDelete);
                deletedTask = new List<Object>();
                productCollection.DeleteOne(Builders<VetTaskModel>.Filter.Eq("_id", ObjectId.Parse(id)));
                
                return RedirectToAction("../AllTasks/Index");
            }
            catch
            {
                return View();
            }
        }

       
    }
}
