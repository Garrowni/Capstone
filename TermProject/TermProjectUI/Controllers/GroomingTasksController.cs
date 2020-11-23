
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

    public class GroomingTasksController : Controller
    {
        // private InventoryTaskDBModelContainer db = new TransportTaskDBModelContainer();

        //private InventoryTaskRepoEF ttr = new InventoryTaskRepoEF();
         

        static List<string> assignees = new List<string>();
        static List<GroomingTaskModel.Comment> comments = new List<GroomingTaskModel.Comment>();
        static GroomingTaskModel.Comment scomm = new GroomingTaskModel.Comment();


        static List<Object> deletedTask = new List<Object>();

        private MongoDBContext dbcontext;
        private IMongoCollection<GroomingTaskModel> productCollection;
        private IMongoCollection<DeletedTaskModel> deletedCollection;
        private IMongoCollection<VolunteerModel> volunteerCollection;

        public GroomingTasksController()
        {
            dbcontext = new MongoDBContext();
            productCollection = dbcontext.database.GetCollection<GroomingTaskModel>("grooming");
            deletedCollection = dbcontext.database.GetCollection<DeletedTaskModel>("deletedTasks");
            volunteerCollection = dbcontext.database.GetCollection<VolunteerModel>("volunteer");
        }
        // GET: TransportationTasks
        public ActionResult Index()
        {
            // return View(db.TransportationTasks.ToList());
            //return View(ttr.GetTransportationTasks());

            List<GroomingTaskModel> tasks = productCollection.AsQueryable<GroomingTaskModel>().ToList();
            return View(tasks);
        }


        // GET: TransportationTasks/Details/5
        public ActionResult Details(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<GroomingTaskModel>().SingleOrDefault(x => x.Id == taskId);
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
                task.Comments = new List<GroomingTaskModel.Comment>();
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
        public ActionResult Create(GroomingTaskModel groomingTask)
        {
            groomingTask.posterName = Session["Username"].ToString();
            groomingTask.posterPhoto = Session["Img"].ToString();
            groomingTask.taskType = "Grooming Task";
            groomingTask.taskName = "Grooming - " + groomingTask.dogName;


            groomingTask.state = "Unassigned";
            var vol = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Name ==groomingTask.requester);
            groomingTask.reqphoto = vol.UserPhoto;
            //    groomingTask.Services = servicesList;


            try
            {
                productCollection.InsertOne(groomingTask);
           //     servicesList = new List<GroomingTaskModel.services>();
                deletedTask = new List<object>();
                deletedTask.Add(groomingTask);
                Session["TaskCount"] = Int32.Parse(Session["TaskCount"].ToString()) + 1;

                return RedirectToAction("Details", new { id = groomingTask.Id });
                
            }
            catch
            {
                return View();
            }
        }

   //     public JsonResult InsertDocuments(List<GroomingTaskModel.services> itemsSpec)
    //    {

            //Check for NULL.
   //         if (itemsSpec == null)
    //        {
    //            itemsSpec = new List<GroomingTaskModel.services>();
    //        }



      //     foreach (GroomingTaskModel.services itemSpec in itemsSpec)
      //      {

     //           servicesList.Add(itemSpec);
//
       //     }
            //Debug.WriteLine(taskSpecList[0].Key);
      //      int insertedRecords = servicesList.Count();
     //       return Json(insertedRecords);

   //     }
   //     public JsonResult UpdateDocuments(List<GroomingTaskModel.services> tasksSpec)
    //    {
//
            //Check for NULL.
   //         if (tasksSpec == null)
   //         {
    //            tasksSpec = new List<GroomingTaskModel.services>();
    //        }

    //        servicesList = new List<GroomingTaskModel.services>();
    //        foreach (GroomingTaskModel.services taskSpec in tasksSpec)
     //       {
    //            servicesList.Add(taskSpec);

     //       }

    //        int insertedRecords = servicesList.Count();
    //        return Json(insertedRecords);

     //   }



        // GET: TransportationTasks/Edit/5
        public ActionResult Edit(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<GroomingTaskModel>().SingleOrDefault(x => x.Id == taskId);
            return View(task);
        }

        // POST: TransportationTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, GroomingTaskModel task)
        {
            
            try
            {
                var filter = Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<GroomingTaskModel>.Update
                    .Set("ImportanceLevel", task.ImportanceLevel)
                     .Set("requester", task.requester)
                    .Set("AdditionalInfo", task.AdditionalInfo)
                    .Set("taskID", task.taskID)
                    .Set("taskName", task.taskName)
                    .Set("taskType", task.taskType)
                    .Set("posterName", task.posterName)
                    .Set("creationDate", task.creationDate)
                    .Set("state", task.state)
                    .Set("dogName", task.dogName)
                    .Set("dogAge", task.dogAge)
                    .Set("dogBreed", task.dogBreed)
                    .Set("dogSize", task.dogSize)
                    //.Set("Services", task.Services)
                    .Set("booked", task.booked)
                    .Set("bookedAddress", task.bookedAddress)
                    .Set("bookedStore", task.bookedStore)
                    .Set("bookedDate", task.bookedDate)
                    .Set("bookedTime", task.bookedTime)
                    .Set("prefStore", task.prefStore)
                    .Set("prefAddress", task.prefAddress)
                    .Set("prefDate", task.prefDate)
                    .Set("paid", task.paid)
                    .Set("price", task.price)
                    .Set("payer", task.payer)
                    .Set("receipt", task.receipt);
                var result = productCollection.UpdateOne(filter, update);
                deletedTask = new List<object>();
                task.Id = ObjectId.Parse(id);
                deletedTask.Add(task);

               // servicesList = new List<GroomingTaskModel.services>();

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
            var task = productCollection.AsQueryable<GroomingTaskModel>().SingleOrDefault(x => x.Id == taskId);
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
                taskDelete.tasksType = "Grooming";
                deletedCollection.InsertOne(taskDelete);
                deletedTask = new List<Object>();
                productCollection.DeleteOne(Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id)));
                
                return RedirectToAction("../AllTasks/Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult JoinTask(string id, GroomingTaskModel task)
        {
            assignees.Add(Session["UserId"].ToString());
            task.assignees = assignees;

            var filter = Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<GroomingTaskModel>.Update
                .Set("assignees", assignees)
                .Set("state", "Assigned");
            var result = productCollection.UpdateOne(filter, update);
            Session["JoinedTaskCount"] = Int32.Parse(Session["JoinedTaskCount"].ToString()) + 1;
            assignees = new List<string>();
            return RedirectToAction("Details", new { id = id });



        }
        public ActionResult DisjointTask(string id, GroomingTaskModel task)
        {
            assignees.Remove(Session["UserId"].ToString());
            Session["JoinedTaskCount"] = Int32.Parse(Session["JoinedTaskCount"].ToString()) - 1;
            if (assignees.Count == 0 || assignees == null)
            {
                task.assignees = assignees;

                var filter = Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<GroomingTaskModel>.Update
                    .Set("assignees", assignees)
                     .Set("state", "Unassigned");
                var result = productCollection.UpdateOne(filter, update);

                assignees = new List<string>();
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                task.assignees = assignees;

                var filter = Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<GroomingTaskModel>.Update
                    .Set("assignees", assignees);
                var result = productCollection.UpdateOne(filter, update);

                assignees = new List<string>();
                return RedirectToAction("Details", new { id = id });
            }



        }
        public ActionResult CompleteTask(string id, GroomingTaskModel task)
        {


            var filter = Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<GroomingTaskModel>.Update
                 .Set("state", "Completed");
            var result = productCollection.UpdateOne(filter, update);
            if (Session["Role"].ToString() == "Admin" || Session["Role"].ToString() == "Moderator")
            {
                Session["TaskCount"] = Int32.Parse(Session["TaskCount"].ToString()) - 1;
              //  Session["JoinedTaskCount"] = Int32.Parse(Session["JoinedTaskCount"].ToString()) - 1;

                Session["CompletedTaskCount"] = Int32.Parse(Session["CompletedTaskCount"].ToString()) + 1;
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

            var task = productCollection.AsQueryable<GroomingTaskModel>().SingleOrDefault(x => x.Id == taskId);
            //task.singleComm = null;
            comments = new List<GroomingTaskModel.Comment>();
            if (task.Comments == null || task.Comments.Count() == 0)
            {
                comments = new List<GroomingTaskModel.Comment>();

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
        public ActionResult AddComment(string id, GroomingTaskModel task)
        {

            scomm.volunteerId = Session["UserId"].ToString();
            scomm.comm = task.singleComm;
            scomm.volunteerName = Session["Username"].ToString();
            scomm.volunteerPhoto = Session["Img"].ToString();
            comments = new List<GroomingTaskModel.Comment>();

            var singletask = productCollection.AsQueryable<GroomingTaskModel>().SingleOrDefault(x => x.Id == new ObjectId(id));

            if (singletask.Comments == null || singletask.Comments.Count() == 0)
            {
                comments = new List<GroomingTaskModel.Comment>();
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
            var filter = Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<GroomingTaskModel>.Update
               .Set("Comments", comments)

                 .Set("singleComm", "");
            var result = productCollection.UpdateOne(filter, update);
            return RedirectToAction("AddComment", new { id = id });
            //return RedirectToAction("Details", new { id = id });

        }
        public ActionResult DeleteComment(string id, GroomingTaskModel task, string comment)
        {
            GroomingTaskModel.Comment comment1 = new GroomingTaskModel.Comment();
            var singletask = productCollection.AsQueryable<GroomingTaskModel>().SingleOrDefault(x => x.Id == new ObjectId(id));

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
            var filter = Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<GroomingTaskModel>.Update
               .Set("Comments", comments);
            var result = productCollection.UpdateOne(filter, update);

            return RedirectToAction("AddComment", new { id = id });
        }
        public ActionResult EditComment(string id, GroomingTaskModel task, string commentId, string comment)
        {

            ViewBag.edit = false;
            var filter = Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<GroomingTaskModel>.Update
               .Set("singleComm", comment);

            //.Set("singleComm", task.singleComm);
            var result = productCollection.UpdateOne(filter, update);
            GroomingTaskModel.Comment comment1 = new GroomingTaskModel.Comment();
            var singletask = productCollection.AsQueryable<GroomingTaskModel>().SingleOrDefault(x => x.Id == new ObjectId(id));

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
            var filter2 = Builders<GroomingTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update2 = Builders<GroomingTaskModel>.Update
               .Set("Comments", comments);
            var result2 = productCollection.UpdateOne(filter2, update2);

            ViewBag.edit = true;
            return RedirectToAction("AddComment", new { id = id });

        }





    }
}
