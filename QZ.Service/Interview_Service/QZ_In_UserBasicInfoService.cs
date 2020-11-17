using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QZ.Interface.Interview_IService;
using QZ.Model.DBContext;
using QZ.Model.Expand;
using QZ.Model.Expand.Interview;
using QZ.Model.Interview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QZ.Service.Interview_Service
{
    public class QZ_In_UserBasicInfoService : BaseService, QZ_In_IUserBasicInfoService
    {
        private readonly DbSet<QZ_Model_In_UserBasicInfo> _UserBasicInfos;
        public QZ_In_UserBasicInfoService(Interview_DB_EFContext dbContext) : base(dbContext)
        {
            this._UserBasicInfos = dbContext.UserBasicInfos;
        }

        #region 读取
        /// <summary>
        /// 根据用户ID获取基本信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public QZ_Model_In_UserBasicInfo GetBasicInfo(int uid)
        {
            return _UserBasicInfos.FirstOrDefault(p => p.UserID == uid);
        }
        #endregion

        #region 写入
        /// <summary>
        /// 提交用户基本信息
        /// </summary>
        /// <param name="info">带保存的基本信息</param>
        /// <param name="basicInfo">返回基本信息</param>
        /// <returns></returns>
        public bool SubmitBasicInfo(Interview_UserBasicInfo info, out QZ_Model_In_UserBasicInfo basicInfo)
        {
            try
            {
                QZ_Model_In_UserBasicInfo model = base.FirstOrDefault<QZ_Model_In_UserBasicInfo>(p => p.UserID == info.UserID);
                if (model != null)
                {
                    //更改
                    model.RealName = info.RealName;
                    model.Gender = info.Gender;
                    model.Nation = info.Nation;
                    model.Education = info.Education;
                    model.IdentityNumber = info.IdentityNumber;
                    model.BirthDate = info.BirthDate;
                    model.Age = info.Age;
                    model.Marriage = info.Marriage;
                    model.NativePlace = info.NativePlace;
                    model.Farmer = info.Farmer;
                    model.Moblie = info.Moblie;
                    model.WechatID = info.WechatID;
                    model.EmergencyContact = info.EmergencyContact;
                    model.EmergencyMobile = info.EmergencyMobile;
                    model.Educations = info.EducationsJson;
                    model.Jobs = info.JobsJson;
                    model.ApplyJob = info.ApplyJob;
                    model.ExpectSalary = info.ExpectSalary;
                    model.LowSalary = info.LowSalary;
                    model.ArriveTime = info.ArriveTime;
                    model.ResumeSource = info.ResumeSource;
                    model.Updatetime = DateTime.Now;
                    base.Update(model);
                }
                else
                {
                    //新增
                    model = new QZ_Model_In_UserBasicInfo();
                    model.UserID = info.UserID;
                    model.RealName = info.RealName;
                    model.Gender = info.Gender;
                    model.Nation = info.Nation;
                    model.Education = info.Education;
                    model.IdentityNumber = info.IdentityNumber;
                    model.BirthDate = info.BirthDate;
                    model.Age = info.Age;
                    model.Marriage = info.Marriage;
                    model.NativePlace = info.NativePlace;
                    model.Farmer = info.Farmer;
                    model.Moblie = info.Moblie;
                    model.WechatID = info.WechatID;
                    model.EmergencyContact = info.EmergencyContact;
                    model.EmergencyMobile = info.EmergencyMobile;
                    model.Educations = info.EducationsJson;
                    model.Jobs = info.JobsJson;
                    model.ApplyJob = info.ApplyJob;
                    model.ExpectSalary = info.ExpectSalary;
                    model.LowSalary = info.LowSalary;
                    model.ArriveTime = info.ArriveTime;
                    model.ResumeSource = info.ResumeSource;
                    model.AddTime = DateTime.Now;
                    model.Updatetime = DateTime.Now;
                    base.Insert(model);
                }
                basicInfo = model;
                return true;
            }
            catch (Exception)
            {
                basicInfo = null;
                return false;
            }
        }

        /// <summary>
        /// 更新用户基本信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateBasicInfo(Interview_UserBasicInfoNew model)
        {
            QZ_Model_In_UserBasicInfo basicInfo = _UserBasicInfos.Find(model.ID);
            if (basicInfo == null)
            {
                return false;
            }
            basicInfo.RealName = model.RealName;
            basicInfo.Gender = model.Gender;
            basicInfo.Nation = model.Nation;
            basicInfo.Education = model.Education;
            basicInfo.IdentityNumber = model.IdentityNumber;
            basicInfo.BirthDate = model.BirthDate;
            basicInfo.Age = model.Age;
            basicInfo.Marriage = model.Marriage;
            basicInfo.NativePlace = model.NativePlace;
            basicInfo.Farmer = model.Farmer;
            basicInfo.Moblie = model.Moblie;
            basicInfo.WechatID = model.WechatID;
            basicInfo.EmergencyContact = model.EmergencyContact;
            basicInfo.EmergencyMobile = model.EmergencyMobile;
            basicInfo.Relation = model.Relation;
            base.Update(basicInfo);
            return true;
        }

        /// <summary>
        /// 更新用户职位及薪资
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateUserApplyJobs(Interview_UserApplyJobs model)
        {
            QZ_Model_In_UserBasicInfo basicInfo = new QZ_Model_In_UserBasicInfo() { ID = model.ID };
            basicInfo.ApplyJob = model.ApplyJob;
            basicInfo.ExpectSalary = model.ExpectSalary;
            basicInfo.LowSalary = model.LowSalary;
            basicInfo.ArriveTime = model.ArriveTime;
            base._DbContext.Attach(basicInfo);
            base._DbContext.Entry(basicInfo).Property(p => p.ApplyJob).IsModified = true;
            base._DbContext.Entry(basicInfo).Property(p => p.ExpectSalary).IsModified = true;
            base._DbContext.Entry(basicInfo).Property(p => p.LowSalary).IsModified = true;
            base._DbContext.Entry(basicInfo).Property(p => p.ArriveTime).IsModified = true;
            return base._DbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// 更新用户教育经历
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateUserEduactions(Interview_UserEducationsNew model)
        {
            QZ_Model_In_UserBasicInfo basicInfo = new QZ_Model_In_UserBasicInfo() { ID = model.ID };
            basicInfo.Educations = JsonConvert.SerializeObject(model.Educations);
            base._DbContext.Attach(basicInfo);
            base._DbContext.Entry(basicInfo).Property(p => p.Educations).IsModified = true;
            return base._DbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// 更新用工作经历
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateUserPastJobs(Interview_UserPastJobs model)
        {
            QZ_Model_In_UserBasicInfo basicInfo = new QZ_Model_In_UserBasicInfo() { ID = model.ID };
            basicInfo.Jobs = JsonConvert.SerializeObject(model.UserPastJobs);
            base._DbContext.Attach(basicInfo);
            base._DbContext.Entry(basicInfo).Property(p => p.Jobs).IsModified = true;
            return base._DbContext.SaveChanges() > 0;
        }
        #endregion
    }
}
