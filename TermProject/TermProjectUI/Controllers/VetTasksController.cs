
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

    public class VetTasksController : Controller
    {
        //static List<VetTaskModel.documents> documentsList = new List<VetTaskModel.documents>();

        static List<string> documentsList = new List<string>();
        static List<string> existedList = new List<string>();

        static List<Object> deletedTask = new List<Object>();
        static List<string> assignees = new List<string>();
        private MongoDBContext dbcontext;
        private IMongoCollection<VetTaskModel>vetCollection;
        private IMongoCollection<DeletedTaskModel> deletedCollection;
        private IMongoCollection<VolunteerModel> volunteerCollection;
        public VetTasksController()
        {
            dbcontext = new MongoDBContext();
            vetCollection = dbcontext.database.GetCollection<VetTaskModel>("vet");
            deletedCollection = dbcontext.database.GetCollection<DeletedTaskModel>("deletedTasks");
            volunteerCollection = dbcontext.database.GetCollection<VolunteerModel>("volunteer");

        }
        // GET: TransportationTasks
        public ActionResult Index()
        {
            // return View(db.TransportationTasks.ToList());
            //return View(ttr.GetTransportationTasks());

            List<VetTaskModel> tasks = vetCollection.AsQueryable<VetTaskModel>().ToList();
            return View(tasks);
        }

        // GET: TransportationTasks/Details/5
        public ActionResult Details(string id)
        {
            var taskId = new ObjectId(id);
            var task = vetCollection.AsQueryable<VetTaskModel>().SingleOrDefault(x => x.Id == taskId);
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
        public ActionResult Create(VetTaskModel vetTask)
        {
            vetTask.posterName = Session["Username"].ToString();
            vetTask.posterPhoto = Session["Img"].ToString();
            vetTask.taskType = "Vet Task";
            vetTask.taskName = "VetTaskTest";
            vetTask.requester = "Ellie";

            vetTask.state = "Unassigned";

            vetTask.FileList = documentsList;

            try
            {
               vetCollection.InsertOne(vetTask);
                deletedTask = new List<object>();
                deletedTask.Add(vetTask);
                documentsList = new List<string>();

                return RedirectToAction("Details", new { id = vetTask.Id });

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
            var task = vetCollection.AsQueryable<VetTaskModel>().SingleOrDefault(x => x.Id == taskId);
            return View(task);
        }

        // POST: TransportationTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, VetTaskModel task)
        {
            task.FileList = documentsList;
            try
            {
                var filter = Builders<VetTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<VetTaskModel>.Update


                    .Set("taskID", task.taskID)
                    .Set("taskName", task.taskName)
                    .Set("taskType", task.taskType)
                    .Set("posterName", task.posterName)
                    .Set("creationDate", task.creationDate)
                    .Set("state", task.state)
                    .Set("AdditionalInfo", task.AdditionalInfo)
                    .Set("pickupLocation", task.pickupLocation)
                    .Set("pickupVolunteer", task.pickupVolunteer)

                    .Set("appointmentAddress", task.appointmentAddress)

               
                    .Set("appointmentReason", task.appointmentReason)
                    .Set("appointmentNotes", task.appointmentNotes)
                    .Set("dropoffLocation", task.dropoffLocation)
                    .Set("dropoffVolunteer", task.dropoffVolunteer)
                    .Set("DODate", task.DODate)
                    .Set("DOTime", task.DOTime)
                    .Set("dogName", task.dogName)
                    .Set("dogBreed", task.dogBreed)
                    .Set("dogSize", task.dogSize)
                    .Set("dogNotes", task.dogNotes)
                    .Set("FileList", task.FileList);

                var result = vetCollection.UpdateOne(filter, update);
                deletedTask = new List<object>();
                task.Id = ObjectId.Parse(id);
                deletedTask.Add(task);
                documentsList = new List<string>();
          //      documentsList = new List<VetTaskModel.documents>();

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
            var task = vetCollection.AsQueryable<VetTaskModel>().SingleOrDefault(x => x.Id == taskId);
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
                taskDelete.tasksType = "Vet";
                deletedCollection.InsertOne(taskDelete);
                deletedTask = new List<Object>();
                vetCollection.DeleteOne(Builders<VetTaskModel>.Filter.Eq("_id", ObjectId.Parse(id)));

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
            return Json("Files uploaded successfully", JsonRequestBehavior.AllowGet);




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
            return Json("Files uploaded successfully", JsonRequestBehavior.AllowGet);




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

        public ActionResult JoinTask(string id, VetTaskModel task)
        {
            assignees.Add(Session["UserId"].ToString());
            task.assignees = assignees;

            var filter = Builders<VetTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<VetTaskModel>.Update
                .Set("assignees", assignees)
                .Set("state", "Assigned");
            var result = vetCollection.UpdateOne(filter, update);

            assignees = new List<string>();
            return RedirectToAction("Details", new { id = id });



        }
        public ActionResult DisjointTask(string id, VetTaskModel task)
        {
            assignees.Remove(Session["UserId"].ToString());
            if (assignees.Count == 0 || assignees == null)
            {
                task.assignees = assignees;

                var filter = Builders<VetTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<VetTaskModel>.Update
                    .Set("assignees", assignees)
                     .Set("state", "Unassigned");
                var result = vetCollection.UpdateOne(filter, update);

                assignees = new List<string>();
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                task.assignees = assignees;

                var filter = Builders<VetTaskModel>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<VetTaskModel>.Update
                    .Set("assignees", assignees);
                var result = vetCollection.UpdateOne(filter, update);

                assignees = new List<string>();
                return RedirectToAction("Details", new { id = id });
            }




        }

    }
}