using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        BigSchoolContext context = new BigSchoolContext();
        public ActionResult Create()
        {
            Course objCourse = new Course();
            objCourse.ListCategory = context.Category.ToList();
            return View(objCourse);
        }
        [Authorize]
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Course objCoure)
        {
            ModelState.Remove("LecturerId");
            if (!ModelState.IsValid)
            {
                objCoure.ListCategory = context.Category.ToList();
                return View("Create", objCoure);
            }
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCoure.LecturerId = user.Id;
            context.Course.Add(objCoure);
            context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currenUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendaces = context.Attendance.Where(C => C.Attendee == currenUser.Id).ToList();
            var course = new List<Course>();
            foreach (Attendance temp in listAttendaces)
            {
                Course objCourse = temp.Course;
                objCourse.LecturerName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LecturerId).Name;
                course.Add(objCourse);
            }
            return View(course);
        }
        public ActionResult Mine()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().
                FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var courses = context.Course.Where(x => x.LecturerId == currentUser.Id && x.DateTime > DateTime.Now).ToList();
            foreach (Course item in courses)
            {
                item.LecturerName = currentUser.Name;
            }
            return View(courses);
        }
        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().
                FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            var listFollowee = context.Following.Where(c => c.FollowerId == currentUser.Id).ToList();
            var listAttendances = context.Attendance.Where(c => c.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (var course in listAttendances)
            {
                foreach (var item in listFollowee)
                {
                    if (item.FolloweeId==course.Course.LecturerId)
                    {
                        Course objCourse = course.Course;
                        objCourse.LecturerName= System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LecturerId).Name;
                        courses.Add(objCourse);
                    }
                }
            }
            return View(courses);
        }
        public ActionResult Edit(int id)
        {
            Course course = context.Course.FirstOrDefault(x => x.Id == id);
            course.ListCategory = context.Category.ToList();
            return View(course);
        }

        [Authorize]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Course objCourse)
        {
            context.Course.AddOrUpdate(objCourse);
            context.SaveChanges();
            objCourse.ListCategory = context.Category.ToList();
            return View(objCourse);
        }

        public ActionResult Delete(int id)
        {
            Course course = context.Course.FirstOrDefault(x => x.Id == id);
            if (course != null)
            {
                return View(course);
            }
            return HttpNotFound();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteItem(int id)
        {
            Course course = context.Course.FirstOrDefault(x => x.Id == id);
            if (course != null)
            {
                context.Course.Remove(course);
                context.SaveChanges();
                return RedirectToAction("Mine");
            }
            return HttpNotFound();
        }
    }
}