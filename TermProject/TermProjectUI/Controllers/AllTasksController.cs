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
        private IMongoCollection<TransportationTaskModel> productCollection;
        private IMongoCollection<OtherTaskModel> otherCollection;
        public AllTasksController()
        {
            dbcontext = new MongoDBContext();
            productCollection = dbcontext.database.GetCollection<TransportationTaskModel>("transportation");
            otherCollection = dbcontext.database.GetCollection<OtherTaskModel>("other");

        }
        // GET: AllTasks
        public ActionResult Index()
        {
            List<TransportationTaskModel> products = productCollection.AsQueryable<TransportationTaskModel>().ToList();
            List<OtherTaskModel> others = otherCollection.AsQueryable<OtherTaskModel>().ToList();

            AllTaskModel mymodel = new AllTaskModel();
            mymodel.TransportationTasks = products;
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
