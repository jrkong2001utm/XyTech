using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace XyTech.Models
{
    public class login
    {
        public login() { }

        [Required(ErrorMessage = "Please enter username.")]
        [DisplayName("Username")]
        public string u_username { get; set; }

        [Required(ErrorMessage = "Please enter password.")]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string u_pwd { get; set; }
    }
}