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
    
    public partial class tb_attendance
    {
        public int a_id { get; set; }
        public string a_floor { get; set; }
        public System.DateTime a_check { get; set; }
    
        public virtual tb_floor tb_floor { get; set; }
    }
}
