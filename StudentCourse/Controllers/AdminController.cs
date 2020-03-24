using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentCourse.Models;

namespace StudentCourse.Controllers
{
    public class AdminController : Controller
    {
        // 对象实体
        sunnyEntities1 db = new sunnyEntities1();

        # region 登录界面显示 +ActionResult Login()
        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }
        #endregion

        #region 验证用户名和密码 +string LoginCheck()
        [HttpPost]
        public string LoginCheck() {
            //获取前台传回的数据
            string username = Request["username"];
            string password = Request["password"];

            //查询数据库 放回 管理员对象
            DbQuery<Models.管理员> query = (from d in db.管理员 where d.管理员名 == username select d) as DbQuery<Models.管理员>;
            List<Models.管理员> list = query.ToList();

            //验证通过，返回成功
            if (list !=null && list.ToArray().Length > 0 && list[0].管理员密码 == password)
            {
                //通过session添加管理员id
                Session["adminId"] = list[0].管理员编号;
                Session["adminName"] = list[0].管理员名;
                return "OK";
            }
            //验证不通过，返回失败
            else
            {

                return "error";
            }
        }
        #endregion

        #region 显示主界面 +ActionResult Main()
        public ActionResult Main() {

            return View();
        }

        #endregion

        #region 显示课程列表页面 +ActionResult Course()
        public ActionResult Course(String id) {

            //查询所有课程信息
            if(id == null || "".Equals(id)) { 
            DateTime Date = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));            
            if (Date.Month > 7) //表示为上学期
            {
                    id = Date.Year + "-" + (Date.Year + 1) + "-" + "1";
                
            }
            else
            {
                    id = (Date.Year - 1) + "-" + Date.Year + "-" + "2";
            }
            }
            DbQuery<Models.课程> courseQuery = (from d in db.课程 where d.备用字段1== id select  d) as DbQuery<Models.课程>;
            List<Models.课程> courseList = courseQuery.ToList();
            
            课程 course = courseList.Count>0?courseList[0]:null;
            if (course == null)
                course = new 课程 { 备用字段1 = id};
            ViewData["DataList"] = courseList;

            //生成 学期下拉框 从 2017 年开始 2017年到现在 Datetime.Now.ToString("yyyyMMddHHmmss")         
            List<SelectListItem> semesterSelectListItems = new List<SelectListItem>();
            DateTime nowDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            DateTime oldDate = DateTime.Parse("2017-08-01");
            while ((nowDate - oldDate).Days >=-180) {
                if (oldDate.Month > 7) //表示为上学期
                {
                    String semester = oldDate.Year + "-" + (oldDate.Year + 1) + "-" + "1";
                    SelectListItem semesterListItem = new SelectListItem { Value = semester, Text = semester };
                    semesterSelectListItems.Add(semesterListItem);
                }
                else
                {
                    String semester = (oldDate.Year-1) + "-" + oldDate.Year + "-" + "2";
                    SelectListItem semesterListItem = new SelectListItem { Value = semester, Text = semester };
                    semesterSelectListItems.Add(semesterListItem);
                }
                oldDate = oldDate.AddMonths(6);
            }

            ViewBag.semesterList = semesterSelectListItems;

            
            return View(course);
        }
        #endregion

        #region 按条件查询 课程列表 
        [HttpPost]
        public ActionResult Course(int id)
        {
            

            return View();
        }
        #endregion
    }
}