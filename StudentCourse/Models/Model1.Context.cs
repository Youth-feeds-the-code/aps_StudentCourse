﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudentCourse.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class sunnyEntities1 : DbContext
    {
        public sunnyEntities1()
            : base("name=sunnyEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<管理员> 管理员 { get; set; }
        public virtual DbSet<教师> 教师 { get; set; }
        public virtual DbSet<课程> 课程 { get; set; }
        public virtual DbSet<通知> 通知 { get; set; }
        public virtual DbSet<选课> 选课 { get; set; }
        public virtual DbSet<学生> 学生 { get; set; }
        public virtual DbSet<院系> 院系 { get; set; }
        public virtual DbSet<专业> 专业 { get; set; }
        public virtual DbSet<时间> 时间 { get; set; }
    }
}
