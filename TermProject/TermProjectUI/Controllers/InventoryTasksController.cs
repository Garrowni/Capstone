
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
        static List<InventoryTaskModel.Comment> comments = new List<InventoryTaskModel.Comment>();
        static InventoryTaskModel.Comment scomm = new InventoryTaskModel.Comment();

        static List<string> assignees = new List<string>();

        static List<Object> deletedTask = new List<Object>();

        private MongoDBContext dbcontext;
        private IMongoCollection<InventoryTaskModel> productCollection;
        private IMongoCollection<DeletedTaskModel> deletedCollection;
        private IMongoCollection<VolunteerModel> volunteerCollection;

        public InventoryTasksController()
        {
            dbcontext = new MongoDBContext();
            productCollection = dbcontext.database.GetCollection<InventoryTaskModel>("inventory");
            deletedCollection = dbcontext.database.GetCollection<DeletedTaskModel>("deletedTasks");
            volunteerCollection = dbcontext.database.GetCollection<VolunteerModel>("volunteer");

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
            ViewBag.req = task.requester;
            ViewBag.post = task.posterName;
            ViewBag.state = task.state;
            ViewBag.comments = task.Comments;
            assignees = new List<string>();
            bool assignedForTask = false;
            if (task.assignees != null)
            {
                List<string> assigneeNames = new List<string>();

                foreach (var assignee in task.assignees)
                {
                    assignees.Add(assignee);
                    if (assignee == Session["UserId"].ToString())
                    {
                        assignedForTask = true;
                    }
                    var volunteerId = new ObjectId(assignee);
                    var volunteer = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Id == volunteerId);
                    assigneeNames.Add(volunteer.Name);
                }
                ViewBag.Message = assignedForTask;
                ViewBag.AssigneeNames = assigneeNames;
            }
            else
            {
                assignees = new List<string>();
                assignedForTask = false;
                ViewBag.Message = assignedForTask;

            }
            if (task.Comments == null)
            {
                task.Comments = new List<InventoryTaskModel.Comment>();
            }
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
            var vol = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Name == inventoryTask.requester);
           inventoryTask.reqPhoto = vol.UserPhoto;

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
            return Json("Your files uploaded successfully", JsonRequestBehavior.AllowGet);




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
            return Json("Your files uploaded successfully", JsonRequestBehavior.AllowGet);




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

        public FileResult Download(string FileName)
        {
            string[] NamePart = FileName.Split('/');
            string lastItem = NamePart[NamePart.Length - 1];
            string[] Names = lastItem.Split('.');
            string extention = Names[Names.Length - 1];
            string Name = Names[0];

            return File(FileName, MimeMapping.GetMimeMapping(FileName), Name + "." + extention);

        }
        public ActionResult JoinTask(string id, InventoryTaskModel task)
        {
            assignees.Add(Session["UserId"].ToString());
            task.assignees = assignees;

            var filter = Builders<InventoryTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<InventoryTaskModel>.Update
                .Set("assignees", assignees)
                .Set("state", "Assigned");
            var result = productCollection.UpdateOne(filter, update);

            assignees = new List<string>();
            return RedirectToAction("Details", new { id = id });



        }
        public ActionResult DisjointTask(string id, InventoryTaskModel task)
        {
            assignees.Remove(Session["UserId"].ToString());
            if (assignees.Count == 0 || assignees == null)
            {
                task.assignees = assignees;

                var filter = Builders<InventoryTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<InventoryTaskModel>.Update
                    .Set("assignees", assignees)
                     .Set("state", "Unassigned");
                var result = productCollection.UpdateOne(filter, update);

                assignees = new List<string>();
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                task.assignees = assignees;

                var filter = Builders<InventoryTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<InventoryTaskModel>.Update
                    .Set("assignees", assignees);
                var result = productCollection.UpdateOne(filter, update);

                assignees = new List<string>();
                return RedirectToAction("Details", new { id = id });
            }



        }
        public ActionResult CompleteTask(string id, InventoryTaskModel task)
        {


            var filter = Builders<InventoryTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<InventoryTaskModel>.Update
                 .Set("state", "Completed");
            var result = productCollection.UpdateOne(filter, update);
            if (Session["Role"].ToString() == "Admin" || Session["Role"].ToString() == "Moderator")
            {
                return RedirectToAction("../CompletedTasks/Index");
            }
            else
            {
                return RedirectToAction("../AllTasks/Index");
            }
        }
        [HttpGet]
        public ActionResult AddComment(string id)
        {
            var taskId = new ObjectId(id);

            var task = productCollection.AsQueryable<InventoryTaskModel>().SingleOrDefault(x => x.Id == taskId);
            //task.singleComm = null;
            comments = new List<InventoryTaskModel.Comment>();
            if (task.Comments == null || task.Comments.Count() == 0)
            {
                comments = new List<InventoryTaskModel.Comment>();

            }
            else
            {
                foreach (var coment in task.Comments)
                {
                    comments.Add(coment);

                }
            }


            return View(task);
        }
        [HttpPost]
        public ActionResult AddComment(string id, InventoryTaskModel task)
        {

            scomm.volunteerId = Session["UserId"].ToString();
            scomm.comm = task.singleComm;
            scomm.volunteerName = Session["Username"].ToString();
            scomm.volunteerPhoto = Session["Img"].ToString();
            comments = new List<InventoryTaskModel.Comment>();

            var singletask = productCollection.AsQueryable<InventoryTaskModel>().SingleOrDefault(x => x.Id == new ObjectId(id));

            if (singletask.Comments == null || singletask.Comments.Count() == 0)
            {
                comments = new List<InventoryTaskModel.Comment>();
                scomm.commId = "1";
            }
            else
            {
                List<int> ids = new List<int>();
                foreach (var coment in singletask.Comments)
                {
                    comments.Add(coment);
                    ids.Add(Int32.Parse(coment.commId));
                }
                scomm.commId = (ids.Max() + 1).ToString();

            }

            comments.Add(scomm);
            // task.singleComm = "";
            var filter = Builders<InventoryTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<InventoryTaskModel>.Update
               .Set("Comments", comments)

                 .Set("singleComm", "");
            var result = productCollection.UpdateOne(filter, update);
            return RedirectToAction("AddComment", new { id = id });
            //return RedirectToAction("Details", new { id = id });

        }
        public ActionResult DeleteComment(string id, InventoryTaskModel task, string comment)
        {
            InventoryTaskModel.Comment comment1 = new InventoryTaskModel.Comment();
            var singletask = productCollection.AsQueryable<InventoryTaskModel>().SingleOrDefault(x => x.Id == new ObjectId(id));

            foreach (var coment in singletask.Comments)
            {

                if (coment.commId == comment)
                {
                    comment1 = coment;
                    // Debug.WriteLine(comment);
                }
            }

            //Debug.WriteLine(comment);
            comments.RemoveAll(l => l.commId == comment);
            // Debug.WriteLine(comments.Count());
            //Debug.WriteLine(comment1.comm);
            var filter = Builders<InventoryTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<InventoryTaskModel>.Update
               .Set("Comments", comments);
            var result = productCollection.UpdateOne(filter, update);

            return RedirectToAction("AddComment", new { id = id });
        }
        public ActionResult EditComment(string id, InventoryTaskModel task, string commentId, string comment)
        {

            ViewBag.edit = false;
            var filter = Builders<InventoryTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<InventoryTaskModel>.Update
               .Set("singleComm", comment);

            //.Set("singleComm", task.singleComm);
            var result = productCollection.UpdateOne(filter, update);
            InventoryTaskModel.Comment comment1 = new InventoryTaskModel.Comment();
            var singletask = productCollection.AsQueryable<InventoryTaskModel>().SingleOrDefault(x => x.Id == new ObjectId(id));

            foreach (var coment in singletask.Comments)
            {

                if (coment.commId == commentId)
                {
                    comment1 = coment;
                    // Debug.WriteLine(comment);
                }
            }

            //Debug.WriteLine(comment);
            comments.RemoveAll(l => l.commId == commentId);
            // Debug.WriteLine(comments.Count());
            //Debug.WriteLine(comment1.comm);
            var filter2 = Builders<InventoryTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update2 = Builders<InventoryTaskModel>.Update
               .Set("Comments", comments);
            var result2 = productCollection.UpdateOne(filter2, update2);

            ViewBag.edit = true;
            return RedirectToAction("AddComment", new { id = id });

        }

    }
}