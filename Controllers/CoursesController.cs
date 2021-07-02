using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
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
    }
}