
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
        static List<TransportationTaskModel.Item> itemList = new List<TransportationTaskModel.Item>();
        static List<string> assignees = new List<string>();
        static List<TransportationTaskModel.Comment> comments = new List<TransportationTaskModel.Comment>();
        static TransportationTaskModel.Comment scomm = new TransportationTaskModel.Comment();
        static List<Object> deletedTask = new List<Object>();
        static List<string> userImage = new List<string>();
       
        private MongoDBContext dbcontext;
        private IMongoCollection<TransportationTaskModel> productCollection;
        private IMongoCollection<DeletedTaskModel> deletedCollection;
        private IMongoCollection<VolunteerModel> volunteerCollection;
        public TransportationTasksController()
        {
            dbcontext = new MongoDBContext();
            productCollection = dbcontext.database.GetCollection<TransportationTaskModel>("transportation");
            deletedCollection = dbcontext.database.GetCollection<DeletedTaskModel>("deletedTasks");
            volunteerCollection = dbcontext.database.GetCollection<VolunteerModel>("volunteer");

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
            ViewBag.req = task.Requester;
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
                task.Comments = new List<TransportationTaskModel.Comment>();
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
        public ActionResult Create(TransportationTaskModel transportationTask)
        {

            transportationTask.posterName = Session["Username"].ToString();
            transportationTask.posterPhoto = Session["Img"].ToString();
            transportationTask.taskType = "Transportation Task";
            transportationTask.taskName = transportationTask.taskType + " - " + transportationTask.PUCity + " to " + transportationTask.DOCity;

            transportationTask.Items = itemList;
            transportationTask.creationDate = DateTime.Today;
            //transportationTask.assignees = "0";
            transportationTask.state = "Unassigned";
            var vol = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Name == transportationTask.Requester);
            transportationTask.reqPhoto = vol.UserPhoto;


            try
            {
                productCollection.InsertOne(transportationTask);
                deletedTask = new List<object>();
                deletedTask.Add(transportationTask);

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
                deletedTask = new List<object>();
                task.Id = ObjectId.Parse(id);
                deletedTask.Add(task);
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
        public ActionResult DeleteConfirmed(string id, DeletedTaskModel taskDelete)
        {
            //  TransportationTask transportationTask = db.TransportationTasks.Find(id);
            //  db.TransportationTasks.Remove(transportationTask);
            //  db.SaveChanges();

            try
            {


                taskDelete.deletedTask = deletedTask;
                taskDelete.tasksType = "Transportation";
                deletedCollection.InsertOne(taskDelete);
                deletedTask = new List<Object>();
                productCollection.DeleteOne(Builders<TransportationTaskModel>.Filter.Eq("_id", ObjectId.Parse(id)));

                return RedirectToAction("../AllTasks/Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult JoinTask(string id, TransportationTaskModel task)
        {
            assignees.Add(Session["UserId"].ToString());
            task.assignees = assignees;

            var filter = Builders<TransportationTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<TransportationTaskModel>.Update
                .Set("assignees", assignees)
                .Set("state", "Assigned");
            var result = productCollection.UpdateOne(filter, update);

            assignees = new List<string>();
            return RedirectToAction("Details", new { id = id });



        }
        public ActionResult DisjointTask(string id, TransportationTaskModel task)
        {
            assignees.Remove(Session["UserId"].ToString());
            if (assignees.Count == 0 || assignees == null)
            {
                task.assignees = assignees;

                var filter = Builders<TransportationTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<TransportationTaskModel>.Update
                    .Set("assignees", assignees)
                     .Set("state", "Unassigned");
                var result = productCollection.UpdateOne(filter, update);

                assignees = new List<string>();
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                task.assignees = assignees;

                var filter = Builders<TransportationTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<TransportationTaskModel>.Update
                    .Set("assignees", assignees);
                var result = productCollection.UpdateOne(filter, update);

                assignees = new List<string>();
                return RedirectToAction("Details", new { id = id });
            }




        }
        public ActionResult CompleteTask(string id, TransportationTaskModel task)
        {


            var filter = Builders<TransportationTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<TransportationTaskModel>.Update
                 .Set("state", "Completed");
            var result = productCollection.UpdateOne(filter, update);
            if (Session["Role"].ToString()=="Admin"|| Session["Role"].ToString() == "Moderator")
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

            var task = productCollection.AsQueryable<TransportationTaskModel>().SingleOrDefault(x => x.Id == taskId);
            //task.singleComm = null;
            comments = new List<TransportationTaskModel.Comment>();
            if (task.Comments == null || task.Comments.Count() == 0)
            {
                comments = new List<TransportationTaskModel.Comment>();

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
        public ActionResult AddComment(string id, TransportationTaskModel task)
        {
            
            scomm.volunteerId = Session["UserId"].ToString();
            scomm.comm = task.singleComm;
            scomm.volunteerName = Session["Username"].ToString();
            scomm.volunteerPhoto = Session["Img"].ToString();
            comments = new List<TransportationTaskModel.Comment>();

            var singletask = productCollection.AsQueryable<TransportationTaskModel>().SingleOrDefault(x => x.Id == new ObjectId(id));
            
            if (singletask.Comments==null || singletask.Comments.Count()==0)
            {
                comments = new List<TransportationTaskModel.Comment>();
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
                scomm.commId = (ids.Max()+1).ToString();

            }
             
            comments.Add(scomm);
           // task.singleComm = "";
            var filter = Builders<TransportationTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<TransportationTaskModel>.Update
               .Set("Comments", comments)
               
                 .Set("singleComm", "");
            var result = productCollection.UpdateOne(filter, update);
            return RedirectToAction("AddComment", new { id = id });
            //return RedirectToAction("Details", new { id = id });

        }
        public ActionResult DeleteComment(string id, TransportationTaskModel task, string comment)
        {
            TransportationTaskModel.Comment comment1 = new TransportationTaskModel.Comment();
            var singletask = productCollection.AsQueryable<TransportationTaskModel>().SingleOrDefault(x => x.Id == new ObjectId(id));
            
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
            var filter = Builders<TransportationTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<TransportationTaskModel>.Update
               .Set("Comments", comments);
            var result = productCollection.UpdateOne(filter, update);

            return RedirectToAction("AddComment", new { id = id });
        }
        public ActionResult EditComment(string id, TransportationTaskModel task, string commentId,string comment)
        {

            ViewBag.edit = false;
           var filter = Builders<TransportationTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<TransportationTaskModel>.Update
               .Set("singleComm", comment);

            //.Set("singleComm", task.singleComm);
            var result = productCollection.UpdateOne(filter, update);
            TransportationTaskModel.Comment comment1 = new TransportationTaskModel.Comment();
            var singletask = productCollection.AsQueryable<TransportationTaskModel>().SingleOrDefault(x => x.Id == new ObjectId(id));

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
            var filter2 = Builders<TransportationTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update2 = Builders<TransportationTaskModel>.Update
               .Set("Comments", comments);
            var result2 = productCollection.UpdateOne(filter2, update2);
            
            ViewBag.edit = true;
            return RedirectToAction("AddComment", new { id = id });

        }

    }
}