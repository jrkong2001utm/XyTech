﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tb_finance> tb_finance { get; set; }
        public virtual DbSet<tb_floor> tb_floor { get; set; }
        public virtual DbSet<tb_inventory> tb_inventory { get; set; }
        public virtual DbSet<tb_investor> tb_investor { get; set; }
        public virtual DbSet<tb_landlord> tb_landlord { get; set; }
        public virtual DbSet<tb_profit> tb_profit { get; set; }
        public virtual DbSet<tb_room> tb_room { get; set; }
        public virtual DbSet<tb_tenant> tb_tenant { get; set; }
        public virtual DbSet<tb_user> tb_user { get; set; }
        public virtual DbSet<tb_attendance> tb_attendance { get; set; }
    }
}
