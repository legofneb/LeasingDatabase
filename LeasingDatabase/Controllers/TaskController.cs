using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    public class TaskController : Controller
    {
        //
        // GET: /Task/

        public ActionResult Index()
        {
            AuleaseEntities db = new AuleaseEntities();
            List<Task> Tasks = db.Tasks.Where(n => n.Status.Id != Status.CompletedTask).ToList();
            ViewBag.Tasks = Tasks;
            
            return View();
        }

        public ActionResult EditTask(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            Task task = db.Tasks.Where(n => n.Id == Id).Single();

            ViewBag.Task = task;
            return View();
        }

        public ActionResult ChangeTask(int Id, string Task, string GID, string Notes)
        {
            AuleaseEntities db = new AuleaseEntities();
            Task task = db.Tasks.Where(n => n.Id == Id).Single();

            task.Name = Task;
            task.Assignee = GID;
            task.Note = Notes;
            db.SaveChanges();

            return RedirectToAction("Index", "Task");
        }

        public ActionResult CloseTask(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            Task task = db.Tasks.Where(n => n.Id == Id).Single();
            task.Status = db.Status.Where(n => n.Id == Status.CompletedTask).Single();
            db.SaveChanges();

            return RedirectToAction("Index", "Task");
        }

        public ActionResult AddTask()
        {
            return View();
        }

        public ActionResult SubmitTask(string Task, string GID, string Notes)
        {
            AuleaseEntities db = new AuleaseEntities();
            Task task = new Task();
            task.Name = Task;
            task.Assignee = GID;
            task.Note = Notes;
            task.Status = db.Status.Where(n => n.Id == Status.NewTask).Single();
            task.Timestamp = DateTime.Now;

            db.Tasks.Add(task);
            db.SaveChanges();
            return RedirectToAction("Index", "Task");
        }

    }
}
