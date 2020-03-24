using StudentCourse.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentCourse.Controllers
{
    //1.控制器
    public class TeacherController : Controller
    {
        


        #region //2.Action方法（可以看成是mvc设计模式的Model） +ActionResult Index()
        public ActionResult Index()
        {
            System.Text.StringBuilder sbhtml = new System.Text.StringBuilder(4000);
            //2.1处理当前业务（比如读取数据库，判断等）
            //2.1.1创建一个数据集合（伪数据），获取数据
            // List<Dog> list = initDate();
            //2.1.2遍历集合，生成html代码,存入 sbhtml
           // list.ForEach(d => {
             //   sbhtml.AppendLine("<div>"+d.ToString()+"</div>");
           // });
            //2.2使用ViewBag传输数据给 同名Index.cshtml 视图
            //ViewBag是一个dynamic类型集合,可以动态添加任意类型的任意名称 的属性和值
           // ViewBag.HtmlStr = sbhtml.ToString();
            return View();
        }
        #endregion

        sunnyEntities1 db = new sunnyEntities1();

        #region 0.2 查询 老师 列表 +ActionResult Abou()
        /// <summary>
        /// 查询 老师 列表
        /// </summary>
        /// <returns></returns>
        public ActionResult TeacherList()
        {
            // 1. 查询 数据库里的 老师数据（通过EF 执行）
            //1.1第一种方式：使用SQO（标准查询运算符）
            //实际放回的 是一个 IQueryable 对象？ 此处其实是放回了一个IQueryable接口的子对象
            //IQueryable<Models.Teacher> query = db.Teacher.Where(d =>d.Deptno == 1 );
            //此时真实 放回的类型为Dbquery<T>，支持延迟加载; 只有当使用到数据库的时候，才去查询数据库！
       //     DbQuery<Models.Teacher> query = db.Teacher.Where(d => d.Deptno == 1) as DbQuery<Models.Teacher> ;
         //   List<Models.Teacher> list = query.ToList();

            //1.2第二种方式：使用Linq语句，查询 所有为软删除的文章
            DbQuery<Models.教师> query = (from d in db.教师 select d) as DbQuery<Models.教师>;
            List<Models.教师> list = query.ToList();

            //2.将集合数据传给视图
            ViewData["DataList"] = list;

            return View();
        }
        #endregion

        #region 0.3执行删除操作（根据id） +ActionResult Del(int id)
        /// <summary>
        /// 执行删除操作（根据id）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Del(int id) {
            try { 
            //1.创建删除的对象
            教师 teacher = new 教师() { 教师编号 = id };
            //2.将对象 添加到EF 管理容器
            db.教师.Attach(teacher);
            //3.将对象包装类的 状态 标识 为删除状态
            db.教师.Remove(teacher);
            //4.更新到数据库 
            db.SaveChanges();
                //5.更新成功，则命令浏览器 重定向 到/Teacher/TeacherList 方法
                return RedirectToAction("TeacherList", "Teacher"); 
            }
            catch (Exception e)
            {
                return Content("删除失败~~" + e.Message);
            }
        }
        #endregion

        #region 0.4显示要修改的数据 +ActionResult Modify(int id)
        [HttpGet]
        public ActionResult Modify(int id)
        {
            //1.根据id 查询 数据库，返回的集合中，拿到 第一个 实体对象
            教师 teacher = (from d in db.教师 where d.教师编号 == id select d).FirstOrDefault();

            //2.1生成 老师 院系  下拉框 列表集合 <option value="1">文本</option>
            IQueryable<院系> query = (from c in db.院系 where c.是否有效 == "是" select c) as IQueryable<院系>;
            List<SelectListItem> listItems = query.Select(c => new SelectListItem { Value=c.院系编号.ToString(),Text = c.院系名}).ToList();
            ViewBag.DeptList = listItems;
            //2.2生成 老师 性别  下拉框 列表集合 <option value="1">文本</option>
            Dictionary<String, String> MaleType = new Dictionary<string, string>();
            MaleType.Add("性别","男");
            Dictionary<String, String> FemaleType = new Dictionary<string, string>();
            FemaleType.Add("性别", "女");
            List<Dictionary<String, String>> SexList = new List<Dictionary<string, string>>();
            SexList.Add(MaleType);
            SexList.Add(FemaleType);
            List<SelectListItem> SexItems = SexList.Select(c => new SelectListItem { Value = c["性别"], Text = c["性别"] }).ToList();
            ViewBag.SexList = SexItems;

            //3. 将teacher 传递给视图显示
            // ViewBag
            // ViewData

            //“加载” 视图，使用view的构造函数，将 数据传给 视图上的 名为Model 的属性
            return View(teacher);
        }
        #endregion

        #region 0.5 执行修改 +ActionResult Modify(教师 model)
        [HttpPost]
        /// <summary>
        /// 执行修改
        /// </summary>
        /// <returns></returns>
        public ActionResult Modify(教师 model)
        {
            try
            {
                //1. 将实体对象 a.加入 EF 对象容器中，并b.获取 伪包装类对象
                DbEntityEntry<教师> entry = db.Entry<教师>(model);
                //2. 将包装类对象的状态设置为 unchanged
                entry.State = System.Data.Entity.EntityState.Unchanged;
                //3.设置 被改变的属性
                entry.Property(a => a.教师姓名).IsModified = true;
                entry.Property(a => a.性别).IsModified = true;
                entry.Property(a => a.院系编号).IsModified = true;

                //4.提交到数据库 完成修改
                db.SaveChanges();
                //5.更新完成，则命令 浏览器 重定向 到 /Teacher/TeacherList 方法
                return RedirectToAction("TeacherList", "Teacher");
            }
            catch (Exception e) {
                return Content("修改失败~"+e.Message);
            }
    }
        #endregion

        #region 0.6显示添加页面 + ActionResult Add()
        [HttpGet]
        public ActionResult Add() {
            //生成 老师 院系  下拉框 列表集合 <option value="1">文本</option>
            IQueryable<院系> query = (from c in db.院系 where c.是否有效 == "是" select c) as IQueryable<院系>;
            List<SelectListItem> listItems = query.Select(c => new SelectListItem { Value = c.院系编号.ToString(), Text = c.院系名 }).ToList();
            ViewBag.DeptList = listItems;
            //生成 老师 性别  下拉框 列表集合 <option value="1">文本</option>
            Dictionary<String, String> MaleType = new Dictionary<string, string>();
            MaleType.Add("性别", "男");
            Dictionary<String, String> FemaleType = new Dictionary<string, string>();
            FemaleType.Add("性别", "女");
            List<Dictionary<String, String>> SexList = new List<Dictionary<string, string>>();
            SexList.Add(MaleType);
            SexList.Add(FemaleType);
            List<SelectListItem> SexItems = SexList.Select(c => new SelectListItem { Value = c["性别"], Text = c["性别"] }).ToList();
            ViewBag.SexList = SexItems;
            return View();
        }
        #endregion

        #region 0.7执行添加操作 
        [HttpPost]
        public ActionResult Add(教师 model) {
            try
            {
                //0.给新实体对象一个唯一的id（教师编号xxxx） 
                var showidmax = db.教师.Select(s => s.教师编号).Max();
                int id = showidmax + 1;
                model.教师编号 = id;
                //1. 将实体对象 a.加入 EF 对象容器中
                db.教师.Add(model);
                //2.提交到数据库 完成添加
                db.SaveChanges();
                //5.更新完成，则命令 浏览器 重定向 到 /Teacher/TeacherList 方法
                return RedirectToAction("TeacherList", "Teacher");           
            }
            catch (Exception e)
            {
                return Content("添加失败~" + e.Message);
            }
        }
        #endregion
    }
}