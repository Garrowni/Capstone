
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
using System.IO;

namespace TermProjectUI.Controllers
{

    public class InventoryTasksController : Controller
    {
        // private InventoryTaskDBModelContainer db = new TransportTaskDBModelContainer();

        //private InventoryTaskRepoEF ttr = new InventoryTaskRepoEF();
        static List<string> documentsList = new List<string>();
        static List<string> existedList = new List<string>();


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
            inventoryTask.posterName = Session["Username"].ToString();
            inventoryTask.posterPhoto = Session["Img"].ToString();
            inventoryTask.taskType = "Inventory Task";
            inventoryTask.taskName = inventoryTask.taskType + " - " + inventoryTask.address;
            inventoryTask.requester = "Ellie";

            inventoryTask.state = "Unassigned";

            inventoryTask.FileList = documentsList;


            try
            {
                productCollection.InsertOne(inventoryTask);

                deletedTask = new List<object>();
                deletedTask.Add(inventoryTask);
                documentsList = new List<string>();

                return RedirectToAction("Details", new { id = inventoryTask.Id });

            }
            catch
            {
                return View();
            }
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
            task.FileList = documentsList;
            task.posterName = Session["Username"].ToString();
            task.posterPhoto = Session["Img"].ToString();
            task.taskType = "Inventory Task";
            task.taskName = task.taskType + " - " + task.address;
            task.requester = "Ellie";

            task.state = "Unassigned";
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
                    .Set("taskDate", task.taskDate)
                    .Set("taskTime", task.taskTime)
                    .Set("AdditionalInfo", task.AdditionalInfo)
                    .Set("FileList", task.FileList); ;
                var result = productCollection.UpdateOne(filter, update);
                deletedTask = new List<object>();
                task.Id = ObjectId.Parse(id);
                deletedTask.Add(task);

                documentsList = new List<string>();

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
        public JsonResult AddFile()
        {

            //string uname = Request["description"];
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                string fname;
                // Checking for Internet Explorer      
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fname = file.FileName;
                }

                // Get the complete folder path and store the file inside it.      
                fname = Path.Combine(Server.MapPath("~/UserImages/"), fname);
                file.SaveAs(fname);
                documentsList.Add("/UserImages/" + file.FileName);
            }
            return Json("Hi. Your files uploaded successfully", JsonRequestBehavior.AllowGet);




        }
        public JsonResult UpdateFile()
        {
            documentsList = new List<string>();
            //string uname = Request["existedList"];
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                string fname;
                // Checking for Internet Explorer      
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fname = file.FileName;
                }
                // Get the complete folder path and store the file inside it.      
                fname = Path.Combine(Server.MapPath("~/UserImages/"), fname);
                file.SaveAs(fname);
                documentsList.Add("/UserImages/" + file.FileName);
            }
            return Json("Hi. Your files uploaded successfully", JsonRequestBehavior.AllowGet);




        }
        public JsonResult ExistedFiles(List<string> items)
        {

            //Check for NULL.
            if (items == null)
            {
                items = new List<string>();
            }


            foreach (string item in items)
            {

                //existedList.Add(item);
                documentsList.Add(item);
            }
            //Debug.WriteLine(itemList[0].ItemName);
            int insertedRecords = existedList.Count();

            return Json(insertedRecords);
        }

        public FileResult Downlaod(string FileName)
        {
            string[] NamePart = FileName.Split('/');
            string lastItem = NamePart[NamePart.Length - 1];
            string[] Names = lastItem.Split('.');
            string extention = Names[Names.Length - 1];
            string Name = Names[0];

            return File(FileName, MimeMapping.GetMimeMapping(FileName), Name + "." + extention);

        }


    }
}