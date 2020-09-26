//OTC
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TermProjectUI.App_Start;
using TermProjectUI.Models;

namespace TermProjectUI.Controllers
{
    public class OtherTasksController : Controller
    {

        static List<OtherTaskModel.TaskRequirement> x = new List<OtherTaskModel.TaskRequirement>();



        private MongoDBContext dbcontext;
        private IMongoCollection<OtherTaskModel> productCollection;

        public OtherTasksController()
        {
            dbcontext = new MongoDBContext();
            productCollection = dbcontext.database.GetCollection<OtherTaskModel>("other");

        }
        // GET: OtherTasks
        public ActionResult Index()
        {
            List<OtherTaskModel> products = productCollection.AsQueryable<OtherTaskModel>().ToList();
            return View(products);
        }

        // GET: OtherTasks/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OtherTasks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OtherTasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OtherTaskModel otherTask)
        {
            otherTask.TaskRequirements = x;
            try
            {
                productCollection.InsertOne(otherTask);
                x = new List<OtherTaskModel.TaskRequirement>();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public JsonResult InsertItems(List<OtherTaskModel.TaskRequirement> customers)
        {

            //Check for NULL.
            if (customers == null)
            {
                customers = new List<OtherTaskModel.TaskRequirement>();
            }

            //Loop and insert records.
            /**foreach (Customer customer in customers)
            {
                entities.Customers.Add(customer);
            }*/
            // List<Item> x = new List<Item>();

            foreach (OtherTaskModel.TaskRequirement customer in customers)
            {

                x.Add(customer);

            }
            Debug.WriteLine(x[0].Key);
            int insertedRecords = x.Count();
            return Json(insertedRecords);

        }

        // GET: OtherTasks/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OtherTasks/Edit/5
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

        // GET: OtherTasks/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OtherTasks/Delete/5
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
