using StudentCourse.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentCourse.Controllers
{
    public class TestController : Controller
    {
        sunnyEntities1 db = new sunnyEntities1();
        // GET: Test
        public ActionResult Index()
        {
            //2.1院系 下拉框
            IQueryable<院系> deptQuery = (from d in db.院系 where d.是否有效 == "是" select d) as IQueryable<院系>;
            List<SelectListItem> deptSelectListItems = deptQuery.Select(c => new SelectListItem { Value = c.院系编号.ToString(), Text = c.院系名 }).ToList();
            ViewBag.DeptList = deptSelectListItems;
            //2.3授课教师 下拉框
            int deptno = int.Parse(deptSelectListItems[0].Value);
            IQueryable<教师> teacherQuery = (from t in db.教师 where t.院系编号 == deptno select t) as IQueryable<教师>;
            List<SelectListItem> teacherSelectListItems = teacherQuery.Select(c => new SelectListItem { Value = c.教师编号.ToString(), Text = c.教师姓名 }).ToList();
            ViewBag.TeachertList = teacherSelectListItems;
            return View();
        }

        [HttpPost]
        public JsonResult About(int id) {

            //2.3授课教师 下拉框
            IQueryable<教师> teacherQuery = (from t in db.教师 where t.院系编号 == id select t) as IQueryable<教师>;
            List<SelectListItem> teacherSelectListItems = teacherQuery.Select(c => new SelectListItem { Value = c.教师编号.ToString(), Text = c.教师姓名 }).ToList();
            
            return Json(teacherSelectListItems, JsonRequestBehavior.DenyGet);
        }
    }
}