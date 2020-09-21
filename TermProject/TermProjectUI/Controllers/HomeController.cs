using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TermProjectUI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }


   


        //create report
     /*   public ActionResult CreateReports()
        {
            ViewBag.Title = "Create Reports";
            return View();
        }*/
        //creating tasks
        //select a tye of new task
        public ActionResult CreateTaskSelection()
        {
            ViewBag.Title = "Create Task Selection";
            return View();
        }


        //create transportation task
        public ActionResult CreateTransportationTask()
        {
            ViewBag.Title = "Create transportation task";
            return View("~/Views/TransportationTasks/Create");
        }
        //create documentation task
       /* public ActionResult CreateDocumentationTask()
        {
            ViewBag.Title = "Create Documentation Task";
            return View();
        }

        //create Donation Task
        public ActionResult CreateDonationTask()
        {
            ViewBag.Title = "CreateDonationTask";
            return View();
        }

        //create maintenance Task
        public ActionResult CreateMaintenanceTask()
        {
            ViewBag.Title = "Create Maintenance Task";
            return View();
        }
        //create maintenance Task
        public ActionResult CreateVetTask()
        {
            ViewBag.Title = "Create Vet Task";
            return View();
        }
        //create Other Task
        public ActionResult CreateOtherTask()
        {
            ViewBag.Title = "Create Other Task";
            return View();
        }*/


        //Viewing


        //View individual Task
        public ActionResult ViewTask()
        {
            ViewBag.Title = "View Task";
            return View();
        }

        //view all tasks
        public ActionResult ViewTasks()
        {
            ViewBag.Title = "View Tasks";
            return View();
        }

        public ActionResult test()
        {
            ViewBag.Title = "Text";
            return View();
        }

    }
}
