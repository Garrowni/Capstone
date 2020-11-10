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
    }
    }

