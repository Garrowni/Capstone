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
    public class CompletedTasksController : Controller
    {
        private MongoDBContext dbcontext;
        private IMongoCollection<TransportationTaskModel> transportationCollection;
        private IMongoCollection<InventoryTaskModel> inventoryCollection;
        private IMongoCollection<PhotographyTaskModel> photographyCollection;
        private IMongoCollection<GroomingTaskModel> groomingCollection;
        private IMongoCollection<VetTaskModel> vetCollection;
        private IMongoCollection<OtherTaskModel> otherCollection;
        public CompletedTasksController()
        {
            dbcontext = new MongoDBContext();
            transportationCollection = dbcontext.database.GetCollection<TransportationTaskModel>("transportation");
            inventoryCollection = dbcontext.database.GetCollection<InventoryTaskModel>("inventory");
            photographyCollection = dbcontext.database.GetCollection<PhotographyTaskModel>("photography");
            groomingCollection = dbcontext.database.GetCollection<GroomingTaskModel>("grooming");
            vetCollection = dbcontext.database.GetCollection<VetTaskModel>("vet");
            otherCollection = dbcontext.database.GetCollection<OtherTaskModel>("other");


        }
        // GET: CompletedTasks
        public ActionResult Index()
        {
            List<TransportationTaskModel> transTasks = new List<TransportationTaskModel>();
            List<TransportationTaskModel> products = transportationCollection.AsQueryable<TransportationTaskModel>().ToList();
            List<InventoryTaskModel> inventoryTasks = new List<InventoryTaskModel>();
            List<InventoryTaskModel> inventory = inventoryCollection.AsQueryable<InventoryTaskModel>().ToList();
            List<PhotographyTaskModel> photographTasks = new List<PhotographyTaskModel>();
            List<PhotographyTaskModel> photography = photographyCollection.AsQueryable<PhotographyTaskModel>().ToList();
            List<GroomingTaskModel> groomingTasks = new List<GroomingTaskModel>();
            List<GroomingTaskModel> grooming = groomingCollection.AsQueryable<GroomingTaskModel>().ToList();
            List<VetTaskModel> vetsTasks = new List<VetTaskModel>();
            List<VetTaskModel> vets = vetCollection.AsQueryable<VetTaskModel>().ToList();
            List<OtherTaskModel> othersTasks = new List<OtherTaskModel>();
            List<OtherTaskModel> others = otherCollection.AsQueryable<OtherTaskModel>().ToList();
            foreach (var trans in products)
            {
                if (trans.state== "Completed")
                {
                   
                            transTasks.Add(trans);
                      
                }


            }
            foreach (var inv in inventory)
            {
                if (inv.state== "Completed")
                {
                  
                            inventoryTasks.Add(inv);
                       
                }


            }
            foreach (var photo in photography)
            {
                if (photo.state== "Completed")
                {
                   
                            photographTasks.Add(photo);
                       
                }

            }
            foreach (var groom in grooming)
            {
                if (groom.state== "Completed")
                {
                   
                            groomingTasks.Add(groom);
                      
                }

            }
            foreach (var vet in vets)
            {
                if (vet.state == "Completed")
                {
                   
                            vetsTasks.Add(vet);
                       
                }

            }
            foreach (var other in others)
            {
                if (other.state=="Completed")
                {
                    
                            othersTasks.Add(other);
                      
                }

            }
           CompletedTasksModel mymodel = new CompletedTasksModel();
            mymodel.TransportationTasks = transTasks;
            mymodel.VetTasks = vetsTasks;
            mymodel.PhotographyTasks = photographTasks;
            mymodel.GroomingTasks = groomingTasks;
            mymodel.InventoryTasks = inventoryTasks;
            mymodel.OtherTasks = othersTasks;

            return View(mymodel);
        }

        // GET: CompletedTasks/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CompletedTasks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CompletedTasks/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CompletedTasks/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CompletedTasks/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CompletedTasks/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CompletedTasks/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
