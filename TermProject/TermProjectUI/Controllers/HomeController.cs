using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TermProjectUI.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;
using System.Net.Mail;
using System.Web.Security;
using System.Windows;
using TermProjectUI.App_Start;


namespace TermProjectUI.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            HomeModel m = new HomeModel();

            ViewBag.Title = "Home Page";


            return View();
        }

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
