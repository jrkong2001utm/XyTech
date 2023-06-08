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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class tb_finance
    {
        public int f_id { get; set; }

        [DisplayName("Floor")]
        [Required(ErrorMessage = "Please Select Floor")]
        public Nullable<int> f_floor { get; set; }
        public int f_user { get; set; }

        [DisplayName("Amount (RM)")]
        [Required(ErrorMessage = "Please Enter Amount")]
        public double f_transaction { get; set; }

        [DisplayName("Details")]
        [Required(ErrorMessage = "Please Enter Transaction Details")]
        public string f_desc { get; set; }

        [DisplayName("Inflow/Outflow")]
        [Required(ErrorMessage = "Please Select Transaction Type")]
        public string f_transactiontype { get; set; }

        [DisplayName("Bank/Cash")]
        [Required(ErrorMessage = "Please Select Payment Method")]
        public string f_paymentmethod { get; set; }

        [DisplayName("Date")]
        [Required(ErrorMessage = "Please Enter Date")]
        public System.DateTime f_date { get; set; }

        [DisplayName("Receipt")]
        public string f_receipt { get; set; }
    
        public virtual tb_user tb_user { get; set; }
        public virtual tb_floor tb_floor { get; set; }
    }
}
