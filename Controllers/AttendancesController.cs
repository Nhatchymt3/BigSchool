using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BigSchool.Controllers
{
    public class AttendancesController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Attend(Course attendanceDto)
        {
            var userID = User.Identity.GetUserId();
            BigSchoolContext context = new BigSchoolContext();
            if (context.Attendance.Any(c=>c.Attendee ==userID&&c.Courseid==attendanceDto.Id))
            {
                //return BadRequest("The attendance alreadly exists");
                context.Attendance.Remove(context.Attendance.SingleOrDefault(c => c.Attendee == userID && c.Courseid == attendanceDto.Id));
                context.SaveChanges();
                return Ok("cancel");
            }
            var attendace = new Attendance() { Courseid = attendanceDto.Id, Attendee = User.Identity.GetUserId() };
            context.Attendance.Add(attendace);
            context.SaveChanges();
            return Ok();
        }
        
       
    }
}
