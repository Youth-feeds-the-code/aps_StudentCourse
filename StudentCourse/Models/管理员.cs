//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class 管理员
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public 管理员()
        {
            this.通知 = new HashSet<通知>();
        }
    
        public int 管理员编号 { get; set; }
        public string 管理员名 { get; set; }
        public string 管理员密码 { get; set; }
        public string 备用字段1 { get; set; }
        public string 备用字段2 { get; set; }
        public string 备用字段3 { get; set; }
        public string 备用字段4 { get; set; }
        public string 备用字段5 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<通知> 通知 { get; set; }
    }
}
