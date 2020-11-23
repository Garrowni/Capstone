using MongoDB.Bson;
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

        static List<OtherTaskModel.TaskRequirement> taskSpecList = new List<OtherTaskModel.TaskRequirement>();

        static List<Object> deletedTask = new List<Object>();
        static List<string> assignees = new List<string>();
        static List<OtherTaskModel.Comment> comments = new List<OtherTaskModel.Comment>();
        static OtherTaskModel.Comment scomm = new OtherTaskModel.Comment();

        private MongoDBContext dbcontext;
        private IMongoCollection<OtherTaskModel> productCollection;
        private IMongoCollection<DeletedTaskModel> deletedCollection;
        private IMongoCollection<VolunteerModel> volunteerCollection;
        public OtherTasksController()
        {
            dbcontext = new MongoDBContext();
            productCollection = dbcontext.database.GetCollection<OtherTaskModel>("other");
            deletedCollection = dbcontext.database.GetCollection<DeletedTaskModel>("deletedTasks");
            volunteerCollection = dbcontext.database.GetCollection<VolunteerModel>("volunteer");

        }
        // GET: OtherTasks
        public ActionResult Index()
        {
           
            List<OtherTaskModel> tasks = productCollection.AsQueryable<OtherTaskModel>().ToList();
            return View(tasks);
            
        }

        // GET: OtherTasks/Details/5
        public ActionResult Details(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<OtherTaskModel>().SingleOrDefault(x => x.Id == taskId);
            ViewBag.req = task.requester;
            ViewBag.comments = task.Comments;
            ViewBag.post = task.posterName;
            ViewBag.state = task.state;
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
                task.Comments = new List<OtherTaskModel.Comment>();
            }
            return View(task);
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
            otherTask.posterName = Session["Username"].ToString();
            otherTask.posterPhoto = Session["Img"].ToString();
            otherTask.TaskRequirements = taskSpecList;
            otherTask.state = "Unassigned";
            otherTask.posterPhoto = Session["Img"].ToString();
            var vol = volunteerCollection.AsQueryable<VolunteerModel>().SingleOrDefault(x => x.Name == otherTask.requester);
           otherTask.reqPhoto = vol.UserPhoto;
            try
            {
                productCollection.InsertOne(otherTask);
                taskSpecList = new List<OtherTaskModel.TaskRequirement>();
                deletedTask = new List<object>();
                deletedTask.Add(otherTask);
                
                return RedirectToAction("Details", new { id = otherTask.Id });
            }
            catch
            {
                return View();
            }
        }
        public JsonResult InsertTaskSpecifications(List<OtherTaskModel.TaskRequirement> itemsSpec)
        {

            //Check for NULL.
            if (itemsSpec == null)
            {
                itemsSpec = new List<OtherTaskModel.TaskRequirement>();
            }

            //Loop and insert records.
            /**foreach (Customer customer in customers)
            {
                entities.Customers.Add(customer);
            }*/
            // List<Item> x = new List<Item>();

            foreach (OtherTaskModel.TaskRequirement itemSpec in itemsSpec)
            {

                taskSpecList.Add(itemSpec);

            }
            //Debug.WriteLine(taskSpecList[0].Key);
            int insertedRecords = taskSpecList.Count();
            return Json(insertedRecords);

        }
        public JsonResult UpdateTaskSpecifications(List<OtherTaskModel.TaskRequirement> tasksSpec)
        {

            //Check for NULL.
            if (tasksSpec == null)
            {
                tasksSpec = new List<OtherTaskModel.TaskRequirement>();
            }

            taskSpecList = new List<OtherTaskModel.TaskRequirement>();
            foreach (OtherTaskModel.TaskRequirement taskSpec in tasksSpec)
            {
                taskSpecList.Add(taskSpec);

            }
           
            int insertedRecords = taskSpecList.Count();
            return Json(insertedRecords);

        }
        // GET: OtherTasks/Edit/5
        public ActionResult Edit(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<OtherTaskModel>().SingleOrDefault(x => x.Id == taskId);
            return View(task);
        }

        // POST: OtherTasks/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, OtherTaskModel task)
        {
            task.TaskRequirements = taskSpecList;
            //task.posterName = "Nicole Garrow";
            try
            {
                var filter = Builders<OtherTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<OtherTaskModel>.Update
                    .Set("requester", task.requester)
                    .Set("ImportanceLevel", task.ImportanceLevel)
                    .Set("posterName", task.posterName)
                    .Set("taskTitle", task.taskTitle)
                    .Set("TaskRequirements", task.TaskRequirements)
                    .Set("AdditionalInfo",task.AdditionalInfo)
                    .Set("assignees",task.assignees)
                    .Set("state", task.state);
                var result = productCollection.UpdateOne(filter, update);
                deletedTask = new List<object>();
                task.Id = ObjectId.Parse(id);
                deletedTask.Add(task);

                taskSpecList = new List<OtherTaskModel.TaskRequirement>();
                return RedirectToAction("Details", new { id = id });
            }
            catch
            {
                return View();
            }
        }
    

        // GET: OtherTasks/Delete/5
        public ActionResult Delete(string id)
        {
            var taskId = new ObjectId(id);
            var task = productCollection.AsQueryable<OtherTaskModel>().SingleOrDefault(x => x.Id == taskId);
            return View(task);
        }

        // POST: OtherTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, DeletedTaskModel taskDelete)
        {
            try
            {
                taskDelete.deletedTask = deletedTask;
                taskDelete.tasksType = "Other";
                deletedCollection.InsertOne(taskDelete);
                deletedTask = new List<Object>();
                productCollection.DeleteOne(Builders<OtherTaskModel>.Filter.Eq("_id", ObjectId.Parse(id)));

                return RedirectToAction("../AllTasks/Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult JoinTask(string id, OtherTaskModel task)
        {
            assignees.Add(Session["UserId"].ToString());
            task.assignees = assignees;

            var filter = Builders<OtherTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<OtherTaskModel>.Update
                .Set("assignees", assignees)
                .Set("state", "Assigned");
            var result = productCollection.UpdateOne(filter, update);

            assignees = new List<string>();
            return RedirectToAction("Details", new { id = id });



        }
        public ActionResult DisjointTask(string id, OtherTaskModel task)
        {
            assignees.Remove(Session["UserId"].ToString());
            if (assignees.Count == 0 || assignees == null)
            {
                task.assignees = assignees;

                var filter = Builders<OtherTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<OtherTaskModel>.Update
                    .Set("assignees", assignees)
                     .Set("state", "Unassigned");
                var result = productCollection.UpdateOne(filter, update);

                assignees = new List<string>();
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                task.assignees = assignees;

                var filter = Builders<OtherTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<OtherTaskModel>.Update
                    .Set("assignees", assignees);
                var result = productCollection.UpdateOne(filter, update);

                assignees = new List<string>();
                return RedirectToAction("Details", new { id = id });
            }

        }
        public ActionResult CompleteTask(string id, OtherTaskModel task)
        {


            var filter = Builders<OtherTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<OtherTaskModel>.Update
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

            var task = productCollection.AsQueryable<OtherTaskModel>().SingleOrDefault(x => x.Id == taskId);
            //task.singleComm = null;
            comments = new List<OtherTaskModel.Comment>();
            if (task.Comments == null || task.Comments.Count() == 0)
            {
                comments = new List<OtherTaskModel.Comment>();

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
        public ActionResult AddComment(string id, OtherTaskModel task)
        {

            scomm.volunteerId = Session["UserId"].ToString();
            scomm.comm = task.singleComm;
            scomm.volunteerName = Session["Username"].ToString();
            scomm.volunteerPhoto = Session["Img"].ToString();
            comments = new List<OtherTaskModel.Comment>();

            var singletask = productCollection.AsQueryable<OtherTaskModel>().SingleOrDefault(x => x.Id == new ObjectId(id));

            if (singletask.Comments == null || singletask.Comments.Count() == 0)
            {
                comments = new List<OtherTaskModel.Comment>();
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
            var filter = Builders<OtherTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<OtherTaskModel>.Update
               .Set("Comments", comments)

                 .Set("singleComm", "");
            var result = productCollection.UpdateOne(filter, update);
            return RedirectToAction("AddComment", new { id = id });
            //return RedirectToAction("Details", new { id = id });

        }
        public ActionResult DeleteComment(string id, OtherTaskModel task, string comment)
        {
            OtherTaskModel.Comment comment1 = new OtherTaskModel.Comment();
            var singletask = productCollection.AsQueryable<OtherTaskModel>().SingleOrDefault(x => x.Id == new ObjectId(id));

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
            var filter = Builders<OtherTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<OtherTaskModel>.Update
               .Set("Comments", comments);
            var result = productCollection.UpdateOne(filter, update);

            return RedirectToAction("AddComment", new { id = id });
        }
        public ActionResult EditComment(string id, OtherTaskModel task, string commentId, string comment)
        {

            ViewBag.edit = false;
            var filter = Builders<OtherTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<OtherTaskModel>.Update
               .Set("singleComm", comment);

            //.Set("singleComm", task.singleComm);
            var result = productCollection.UpdateOne(filter, update);
            OtherTaskModel.Comment comment1 = new OtherTaskModel.Comment();
            var singletask = productCollection.AsQueryable<OtherTaskModel>().SingleOrDefault(x => x.Id == new ObjectId(id));

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
            var filter2 = Builders<OtherTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update2 = Builders<OtherTaskModel>.Update
               .Set("Comments", comments);
            var result2 = productCollection.UpdateOne(filter2, update2);

            ViewBag.edit = true;
            return RedirectToAction("AddComment", new { id = id });

        }

    }
}

