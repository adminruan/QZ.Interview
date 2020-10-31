using Microsoft.EntityFrameworkCore;
using QZ.Model.Interview;
using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Model.DBContext
{
    public class Interview_DB_EFContext : DbContext
    {
        public Interview_DB_EFContext(DbContextOptions<Interview_DB_EFContext> options) : base(options)
        {
            //this.Database.SetCommandTimeout(120000); //时间单位是毫秒
        }

        public virtual DbSet<QZ_Model_In_User> Users { set; get; }
        public virtual DbSet<QZ_Model_In_UserBasicInfo> UserBasicInfos { get; set; }
        public virtual DbSet<QZ_Model_In_AdminInfo> AdminInfos { get; set; }
        public virtual DbSet<QZ_Model_In_Menu> Menus { get; set; }
        public virtual DbSet<QZ_Model_In_InterviewRecords> InterviewRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
