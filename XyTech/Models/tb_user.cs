//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace XyTech.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tb_user
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tb_user()
        {
            this.tb_finance = new HashSet<tb_finance>();
            this.tb_investor = new HashSet<tb_investor>();
        }
    
        public int u_id { get; set; }
        public string u_pwd { get; set; }
        public string u_email { get; set; }
        public string u_phone { get; set; }
        public string u_usertype { get; set; }
        public string u_active { get; set; }
        public string u_username { get; set; }
        public string u_token { get; set; }
        public Nullable<System.DateTime> u_resetdate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_finance> tb_finance { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_investor> tb_investor { get; set; }
    }
}
