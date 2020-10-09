using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TermProjectUI.App_Start;
using TermProjectUI.Models;

namespace TermProjectUI.Controllers
{
    public class AllTasksController : Controller
    {
        private MongoDBContext dbcontext;
        private IMongoCollection<TransportationTaskModel> transportationCollection;
        private IMongoCollection<GroomingTaskModel> groomingCollection;
        private IMongoCollection<InventoryTaskModel> inventoryCollection;
        private IMongoCollection<PhotographyTaskModel> photographyCollection;
        private IMongoCollection<VetTaskModel> vetCollection;
        private IMongoCollection<OtherTaskModel> otherCollection;
        public AllTasksController()
        {
            dbcontext = new MongoDBContext();
            transportationCollection = dbcontext.database.GetCollection<TransportationTaskModel>("transportation");
            groomingCollection = dbcontext.database.GetCollection<GroomingTaskModel>("grooming");
            photographyCollection = dbcontext.database.GetCollection<PhotographyTaskModel>("photography");
            inventoryCollection = dbcontext.database.GetCollection<InventoryTaskModel>("inventory");
            vetCollection = dbcontext.database.GetCollection<VetTaskModel>("vet");
            otherCollection = dbcontext.database.GetCollection<OtherTaskModel>("other");

        }
        // GET: AllTasks
        public ActionResult Index()
        {
            List<TransportationTaskModel> transportations = transportationCollection.AsQueryable<TransportationTaskModel>().ToList();
            List<GroomingTaskModel> grooming = groomingCollection.AsQueryable<GroomingTaskModel>().ToList();
            List<PhotographyTaskModel> photograhy = photographyCollection.AsQueryable<PhotographyTaskModel>().ToList();
            List<InventoryTaskModel> inventory = inventoryCollection.AsQueryable<InventoryTaskModel>().ToList();
            List<VetTaskModel> vet = vetCollection.AsQueryable<VetTaskModel>().ToList();
            List<OtherTaskModel> others = otherCollection.AsQueryable<OtherTaskModel>().ToList();

            AllTaskModel mymodel = new AllTaskModel();
            mymodel.TransportationTasks = transportations;
            mymodel.GroomingTasks = grooming;
            mymodel.PhotographyTasks = photograhy;
            mymodel.InventoryTasks = inventory;
            mymodel.VetTasks = vet;
            mymodel.OtherTasks = others;
            return View(mymodel);
        }



      

        // GET: AllTasks/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AllTasks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AllTasks/Create
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

        // GET: AllTasks/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AllTasks/Edit/5
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

        // GET: AllTasks/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AllTasks/Delete/5
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
