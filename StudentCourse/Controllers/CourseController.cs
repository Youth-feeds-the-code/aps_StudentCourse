using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentCourse.Models;

namespace StudentCourse.Controllers
{
    public class CourseController : Controller
    {
        // 对象实体
        sunnyEntities1 db = new sunnyEntities1();

        #region 显示修改课程界面 +ActionResult Modify(int id)
        [HttpGet]
        public ActionResult Modify(int id)
        {
            //1.根据id 查询 数据库， 放回的集合中，拿到第一个 实体对象
            课程 course = (from d in db.课程 where d.课程编号 == id select d).FirstOrDefault();
            //2.构造 下拉框列表集合
            //2.1院系 下拉框
            IQueryable<院系> deptQuery = (from d in db.院系 where d.是否有效 == "是" select d) as IQueryable<院系>;
            List<SelectListItem> deptSelectListItems = deptQuery.Select(c => new SelectListItem { Value=c.院系编号.ToString(),Text = c.院系名 }).ToList();
            ViewBag.DeptList = deptSelectListItems;
            //2.2学分 下拉框 
            List<SelectListItem> creditSelectListItems = new List<SelectListItem>();
            for (int i = 1;i<=6;i++)
            { 
                 SelectListItem creditselectListItem = new SelectListItem {Value= i.ToString(),Text= i.ToString() };
                creditSelectListItems.Add(creditselectListItem);
            }
            ViewBag.creditList =creditSelectListItems;
            //2.3授课教师 下拉框
            IQueryable<教师> teacherQuery = (from t in db.教师 where t.院系编号 == course.院系编号 select t) as IQueryable<教师>;
            List<SelectListItem> teacherSelectListItems = teacherQuery.Select(c=> new SelectListItem {Value=c.教师编号.ToString(), Text=c.教师姓名 }).ToList();
            ViewBag.teacherList = teacherSelectListItems;
            //2.4课程类型 下拉框
            SelectListItem courseTypeListItem1 = new SelectListItem { Value = "必修", Text = "必修" };
            SelectListItem courseTypeListItem2 = new SelectListItem { Value = "专选", Text = "专选" };
            SelectListItem courseTypeListItem3 = new SelectListItem { Value = "公选", Text = "公选" };
            List<SelectListItem> courseTypeSelectListItems = new List<SelectListItem>();
            courseTypeSelectListItems.Add(courseTypeListItem1);
            courseTypeSelectListItems.Add(courseTypeListItem2);
            courseTypeSelectListItems.Add(courseTypeListItem3);
            ViewBag.courseTypeList = courseTypeSelectListItems;
            //2.5学时类型 下拉框
            List<SelectListItem> periodSelectListItems = new List<SelectListItem>();
            for (int i = 4;i<=64;) {
                SelectListItem periodListItem = new SelectListItem { Value = i.ToString(), Text = i.ToString() };
                periodSelectListItems.Add(periodListItem);
                if (i < 32)
                {
                    i = 2 * i;
                }
                else if (i <= 64)
                {
                    i = i + 16;
                }
            }
            ViewBag.periodList = periodSelectListItems;
            //2.6生成 学期下拉框 从 2017 年开始 2017年到现在 Datetime.Now.ToString("yyyyMMddHHmmss")         
            List<SelectListItem> semesterSelectListItems = new List<SelectListItem>();
            DateTime nowDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            DateTime oldDate = DateTime.Parse("2017-08-01");
            while ((nowDate - oldDate).Days >= -180)
            {
                if (oldDate.Month > 7) //表示为上学期
                {
                    String semester = oldDate.Year + "-" + (oldDate.Year + 1) + "-" + "1";
                    SelectListItem semesterListItem = new SelectListItem { Value = semester, Text = semester };
                    semesterSelectListItems.Add(semesterListItem);
                }
                else
                {
                    String semester = (oldDate.Year - 1) + "-" + oldDate.Year + "-" + "2";
                    SelectListItem semesterListItem = new SelectListItem { Value = semester, Text = semester };
                    semesterSelectListItems.Add(semesterListItem);
                }
                oldDate = oldDate.AddMonths(6);
            }

            ViewBag.semesterList = semesterSelectListItems;

            return View(course);
        }
        #endregion

        #region 执行修改操作 +ActionResult Modify(课程 course)
        [HttpPost]
        public ActionResult Modify(课程 course) {

            String semester = course.备用字段1;
            HttpPostedFileBase file = Request.Files[0];
                    if (file.ContentLength > 0)
                    {
                        string title = string.Empty;
                        String originalFilename = Path.GetFileName(file.FileName);                       
                        title = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + originalFilename.Substring(originalFilename.LastIndexOf("."));
                        //设置物理路劲
                        string path = "E:\\StudentCourseDevelop\\upload\\image\\" + title;                     
                        //将内存中的文件写入磁盘
                        file.SaveAs(path);
                        //保存 路径 到 课程
                        course.课程图片 = title;
                    }
                
            
                try
            {
                //1. 将实体对象 a.加入 EF 对象容器中，并b.获取 伪包装类对象
                DbEntityEntry<课程> entry =  db.Entry<课程>(course);
                //2. 将包装类对象的状态设置为 unchanged
                entry.State = System.Data.Entity.EntityState.Unchanged;
                //3.设置 被改变的属性
                entry.Property(a => a.课程名称).IsModified = true;
                entry.Property(a => a.授课老师).IsModified = true;
                entry.Property(a => a.学分).IsModified = true;
                entry.Property(a => a.课程分类).IsModified = true;
                entry.Property(a => a.限制人数).IsModified = true;
                entry.Property(a => a.开课对象).IsModified = true;
                entry.Property(a => a.上课时间).IsModified = true;
                entry.Property(a => a.上课地点).IsModified = true;
                entry.Property(a => a.课程简介).IsModified = true;
                entry.Property(a => a.院系编号).IsModified = true;
                entry.Property(a => a.课时).IsModified = true;
                entry.Property(a => a.备用字段1).IsModified = true;
                //如果上传 图片 则修改 课程图片
                if (file.ContentLength > 0)
                {
                    entry.Property(a => a.课程图片).IsModified = true;
                }
                //4.提交到数据库 完成修改
                    db.SaveChanges();
                //5.更新完成，则命令 浏览器 重定向 到 /Home/List 方法
                return RedirectToAction("Course/"+semester, "Admin");
            }
            catch (Exception e)
            {
                return Content("修改失败~" + e.Message);
            }
        }
        #endregion


        #region 执行删除课程操作 +ctionResult Del(int id)
        [HttpGet]
        public ActionResult Del(int id) {
            try {             
                //1.创建删除的对象
                课程 course = new 课程() { 课程编号 = id };
                //2.将对象添加到EF 管理容器
                db.课程.Attach(course);
                //3.将对象包装类的 状态 标识为删除状态
                db.课程.Remove(course);
                //4.更新到数据库
                db.SaveChanges();
                //5.更新成功，则命令浏览器 重定向到 /Admin/Course 方法
                return RedirectToAction("Course","Admin");

            }catch (Exception e) {
                return Content("删除失败--"+e.Message);
            }       
        }
        #endregion

        #region 显示添加课程页面 +ActionResult Add() 
        [HttpGet]
        public ActionResult Add() {

            //2.构造 下拉框列表集合
            //2.1院系 下拉框
            IQueryable<院系> deptQuery = (from d in db.院系 where d.是否有效 == "是" select d) as IQueryable<院系>;
            List<SelectListItem> deptSelectListItems = deptQuery.Select(c => new SelectListItem { Value = c.院系编号.ToString(), Text = c.院系名 }).ToList();
            ViewBag.DeptList = deptSelectListItems;
            //2.2学分 下拉框 
            List<SelectListItem> creditSelectListItems = new List<SelectListItem>();
            for (int i = 1; i <= 6; i++)
            {
                SelectListItem creditselectListItem = new SelectListItem { Value = i.ToString(), Text = i.ToString() };
                creditSelectListItems.Add(creditselectListItem);
            }
            ViewBag.creditList = creditSelectListItems;
            //2.3授课教师 下拉框 需求 页面返回 院系编号时，刷新授课老师下拉框
            int deptid = 1;
            IQueryable<教师> teacherQuery = (from t in db.教师 where t.院系编号 == deptid select t) as IQueryable<教师>;
            List<SelectListItem> teacherSelectListItems = teacherQuery.Select(c => new SelectListItem { Value = c.教师编号.ToString(), Text = c.教师姓名 }).ToList();
            ViewBag.teacherList = teacherSelectListItems;
            //2.4课程类型 下拉框
            SelectListItem courseTypeListItem1 = new SelectListItem { Value = "必修", Text = "必修" };
            SelectListItem courseTypeListItem2 = new SelectListItem { Value = "专选", Text = "专选" };
            SelectListItem courseTypeListItem3 = new SelectListItem { Value = "公选", Text = "公选" };
            List<SelectListItem> courseTypeSelectListItems = new List<SelectListItem>();
            courseTypeSelectListItems.Add(courseTypeListItem1);
            courseTypeSelectListItems.Add(courseTypeListItem2);
            courseTypeSelectListItems.Add(courseTypeListItem3);
            ViewBag.courseTypeList = courseTypeSelectListItems;
            //2.5学时类型 下拉框
            List<SelectListItem> periodSelectListItems = new List<SelectListItem>();
            for (int i = 4; i <= 64;)
            {
                SelectListItem periodListItem = new SelectListItem { Value = i.ToString(), Text = i.ToString() };
                periodSelectListItems.Add(periodListItem);
                if (i < 32)
                {
                    i = 2 * i;
                }
                else if (i <= 64)
                {
                    i = i + 16;
                }
            }
            ViewBag.periodList = periodSelectListItems;
            //2.6生成 学期下拉框 从 2017 年开始 2017年到现在 Datetime.Now.ToString("yyyyMMddHHmmss")         
            List<SelectListItem> semesterSelectListItems = new List<SelectListItem>();
            DateTime nowDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            DateTime oldDate = DateTime.Parse("2017-08-01");
            while ((nowDate - oldDate).Days >= -180)
            {
                if (oldDate.Month > 7) //表示为上学期
                {
                    String semester = oldDate.Year + "-" + (oldDate.Year + 1) + "-" + "1";
                    SelectListItem semesterListItem = new SelectListItem { Value = semester, Text = semester };
                    semesterSelectListItems.Add(semesterListItem);
                }
                else
                {
                    String semester = (oldDate.Year - 1) + "-" + oldDate.Year + "-" + "2";
                    SelectListItem semesterListItem = new SelectListItem { Value = semester, Text = semester };
                    semesterSelectListItems.Add(semesterListItem);
                }
                oldDate = oldDate.AddMonths(6);
            }

            ViewBag.semesterList = semesterSelectListItems;


            return View();
        }
        #endregion

        #region 执行添加 课程 操作 +ActionResult Add(课程 course)
        public ActionResult Add(课程 course) {
            HttpPostedFileBase file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                string title = string.Empty;
                String originalFilename = Path.GetFileName(file.FileName);
                title = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + originalFilename.Substring(originalFilename.LastIndexOf("."));
                //设置物理路劲
                string path = "E:\\StudentCourseDevelop\\upload\\image\\" + title;
                //将内存中的文件写入磁盘
                file.SaveAs(path);
                //保存 路径 到 课程
                course.课程图片 = title;
            }

            try
            {
                //0.给新实体对象一个唯一的id（课程编号xxxx） 
                var showidmax = db.课程.Select(s => s.课程编号).Max();
                int id = showidmax + 1;
                course.课程编号 = id;
                //1. 将实体对象 a.加入 EF 对象容器中
                db.课程.Add(course);
                //2.提交到数据库 完成添加
                db.SaveChanges();
                //5.更新完成，则命令 浏览器 重定向 到 /Home/List 方法
                return RedirectToAction("Course/" + course.备用字段1, "Admin");
            }
            catch (Exception e)
            {
                return Content("添加失败~" + e.Message);
            }
        }
        #endregion

        #region 院系 与 老师  下拉框 联动 +JsonResult deptTeacherLinkage(int id)
        [HttpPost]
        public JsonResult deptTeacherLinkage(int id)
        {

            //2.3授课教师 下拉框
            IQueryable<教师> teacherQuery = (from t in db.教师 where t.院系编号 == id select t) as IQueryable<教师>;
            List<SelectListItem> teacherSelectListItems = teacherQuery.Select(c => new SelectListItem { Value = c.教师编号.ToString(), Text = c.教师姓名 }).ToList();

            return Json(teacherSelectListItems, JsonRequestBehavior.DenyGet);
        }
        #endregion
    }
}