using Microsoft.EntityFrameworkCore;
using QZ.Common.Enums;
using QZ.Common.Expand;
using QZ.Interface.Interview_IService;
using QZ.Model.DBContext;
using QZ.Model.Interview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QZ.Service.Interview_Service
{
    public class QZ_In_InterviewRecordsService : BaseService, QZ_In_IInterviewRecordsService
    {
        private readonly DbSet<QZ_Model_In_InterviewRecords> _InterviewRecords;
        private readonly DbSet<QZ_Model_In_User> _Users;
        private readonly DbSet<QZ_Model_In_UserBasicInfo> _UserBasicInfos;
        private readonly DbSet<QZ_Model_In_Positions> _Positions;
        public QZ_In_InterviewRecordsService(Interview_DB_EFContext dbContext) : base(dbContext)
        {
            this._InterviewRecords = dbContext.InterviewRecords;
            this._Users = dbContext.Users;
            this._UserBasicInfos = dbContext.UserBasicInfos;
            this._Positions = dbContext.Positions;
        }

        #region 写入
        /// <summary>
        /// 提交面试记录
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="adminID">负责处理管理员ID</param>
        /// <param name="applyJob">应聘岗位</param>
        /// <returns></returns>
        public bool SubmitInterviewRecord(int uid, int adminID, int applyJob)
        {
            QZ_Model_In_InterviewRecords record = new QZ_Model_In_InterviewRecords();
            record.UserID = uid;
            record.Schedule = (int)QZ_Enum_Schedules.AwaitPlan;
            record.AddTime = DateTime.Now;
            record.InterviewerAdminIds = $"{adminID}|";
            record.ApplyJob = applyJob;
            return base.Insert(record).ID > 0;
        }

        /// <summary>
        /// 分配下轮面试人
        /// </summary>
        /// <param name="records">面试申请记录</param>
        /// <param name="adminID">面试官ID</param>
        /// <param name="schedules">下轮面进度</param>
        /// <returns></returns>
        public bool ArrangeInterviewer(QZ_Model_In_InterviewRecords records, int adminID, QZ_Enum_Schedules schedules)
        {
            try
            {
                records.Schedule = (int)schedules;
                records.InterviewerAdminIds += $"{adminID}|";
                base._DbContext.Attach(records);
                base._DbContext.Entry(records).Property(p => p.InterviewerAdminIds).IsModified = true;
                base._DbContext.Entry(records).Property(p => p.Schedule).IsModified = true;
                return base._DbContext.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }


        #endregion

        #region 读取
        ///// <summary>
        ///// 获取我负责的面试处理面试信息
        ///// </summary>
        ///// <param name="page">当前页</param>
        ///// <param name="limit">当前行数</param>
        ///// <param name="adminID">管理员ID</param>
        ///// <param name="isUsed">查询类型（True：已处理、False：待处理）</param>
        ///// <param name="total">总行数</param>
        ///// <param name="schedule">查询已处理，可指定筛选状态（4：不适合、5：备用、6：适合）</param>
        ///// <returns></returns>
        //public List<QZ_Model_In_UserBasicInfo> GetInterviews(int page, int limit, int adminID, bool isUsed, out int total, byte? schedule)
        //{
        //    Expression<Func<QZ_Model_In_UserBasicInfo, bool>> where = p => true;
        //    if (isUsed)
        //        where = where.And(p => new byte[] { 4, 5, 6 }.Contains(p.ExtSchedule));
        //    else
        //    {
        //        if (schedule != null)
        //        {
        //            where = where.And(p => schedule == p.ExtSchedule);
        //        }
        //        else
        //        {
        //            where = where.And(p => !new byte[] { 4, 5, 6 }.Contains(p.ExtSchedule));
        //        }
        //    }
        //    var data = (from i in _InterviewRecords
        //                join u in _Users on i.UserID equals u.UserID
        //                join b in _UserBasicInfos on u.UserID equals b.UserID
        //                where i.InterviewerAdminIds.Contains(adminID.ToString())
        //                select new QZ_Model_In_UserBasicInfo
        //                {
        //                    RealName = b.RealName,
        //                    Gender = b.Gender,
        //                    Age = b.Age,
        //                    BirthDate = b.BirthDate,
        //                    Education = b.Education,
        //                    ApplyJob = b.ApplyJob,
        //                    ExtInterviewDate = i.AddTime,
        //                    ExtSchedule = i.Schedule,
        //                    ExtInterviewID = i.ID
        //                }).Where(where).OrderByDescending(p => p.ExtInterviewDate);
        //    if (!data.Any())
        //    {
        //        total = 0;
        //        return null;
        //    }
        //    total = data.Count();
        //    return data.Skip((page - 1) * limit).Take(limit).ToList();
        //}

        /// <summary>
        /// 获取面试信息
        /// </summary>
        /// <returns></returns>
        public IQueryable<QZ_Model_In_UserBasicInfo> GetDataInterview()
        {
            return (from i in _InterviewRecords
                    join u in _Users on i.UserID equals u.UserID
                    join b in _UserBasicInfos on u.UserID equals b.UserID
                    select new QZ_Model_In_UserBasicInfo
                    {
                        RealName = b.RealName,
                        Gender = b.Gender,
                        Age = b.Age,
                        BirthDate = b.BirthDate,
                        Education = b.Education,
                        ApplyJob = b.ApplyJob,
                        ExtInterviewDate = i.AddTime,
                        ExtSchedule = i.Schedule,
                        ExtInterviewID = i.ID,
                        ExtAdminIds = i.InterviewerAdminIds,
                        UserID = b.UserID,
                        ExtRemarks = i.Remarks,
                        Jobs = b.Jobs,
                        Educations = b.Educations
                    }).OrderByDescending(p => p.ExtInterviewDate);
        }

        /// <summary>
        /// 获取面试信息
        /// 后台管理使用
        /// </summary>
        /// <returns></returns>
        public IQueryable<QZ_Model_In_UserBasicInfo> GetData()
        {
            return (from i in _InterviewRecords
                    join u in _Users on i.UserID equals u.UserID
                    join b in _UserBasicInfos on u.UserID equals b.UserID
                    select new QZ_Model_In_UserBasicInfo
                    {
                        RealName = b.RealName,
                        Gender = b.Gender,
                        Age = b.Age,
                        Moblie = b.Moblie,
                        ApplyJob = b.ApplyJob,
                        ExtSchedule = i.Schedule,
                        ResumeSource = b.ResumeSource,
                        ExtInterviewDate = i.AddTime,
                        ExtRemarks = i.Remarks,
                        ExtArriveTime = b.ArriveTime,
                        ExtInterviewID = i.ID,
                        IdentityNumber = b.IdentityNumber
                    }).OrderByDescending(p => p.ExtInterviewDate);
        }

        /// <summary>
        /// 通过面试记录ID获取用户面试信息
        /// </summary>
        /// <param name="id">面试记录ID</param>
        /// <returns></returns>
        public QZ_Model_In_UserBasicInfo GetInterviewInfoByInterviewID(int id)
        {
            return (from i in _InterviewRecords
                    join b in _UserBasicInfos on i.UserID equals b.UserID
                    join p in _Positions on i.ApplyJob equals p.ID into p2
                    from p in p2.DefaultIfEmpty()
                    where i.ID == id
                    select new QZ_Model_In_UserBasicInfo
                    {
                        RealName = b.RealName,
                        Gender = b.Gender,
                        Age = b.Age,
                        Marriage = b.Marriage,
                        Nation = b.Nation,
                        BirthDate = b.BirthDate,
                        Education = b.Education,
                        Farmer = b.Farmer,
                        NativePlace = b.NativePlace,
                        IdentityNumber = b.IdentityNumber,
                        ExtApplyJob = p.PositionName,
                        ResumeSource = b.ResumeSource,
                        Moblie = b.Moblie,
                        WechatID = b.WechatID,
                        ExtSchedule = i.Schedule,
                        EmergencyContact = b.EmergencyContact,
                        EmergencyMobile = b.EmergencyMobile,
                        Educations = b.Educations,
                        Jobs = b.Jobs,
                        ArriveTime = b.ArriveTime,
                        ExpectSalary = b.ExpectSalary,
                        LowSalary = b.LowSalary,
                        ExtRemarks = i.Remarks,
                        UserID = b.UserID,
                        ExtInterviewDate = i.AddTime,
                        ExtInterviewID = i.ID
                    }).OrderByDescending(p => p.ExtInterviewDate).FirstOrDefault();
        }

        /// <summary>
        /// 获取用户除此面试记录以外的历史面试记录
        /// </summary>
        /// <param name="interviewID">本次面试记录ID</param>
        /// <param name="uid">用户ID</param>
        /// <returns></returns>
        public List<QZ_Model_In_UserBasicInfo> GetHistoryInterviews(int interviewID, int uid)
        {
            return _InterviewRecords.Where(p => p.ID != interviewID && p.UserID == uid).Select(p => new QZ_Model_In_UserBasicInfo
            {
                ExtInterviewDate = p.AddTime,
                ExtRemarks = p.Remarks,
                ExtInterviewID = p.ID,
                ApplyJob = p.ApplyJob,
                ExtSchedule = p.Schedule
            }).OrderByDescending(p => p.ExtInterviewDate).ToList();
        }

        /// <summary>
        /// 获取今日面试人员ixnxi
        /// </summary>
        /// <returns></returns>
        public List<QZ_Model_In_InterviewRecords> GetTodayInterviewInfo()
        {
            return _InterviewRecords.Where(p => p.AddTime >= DateTime.Now.Date).Select(p => new QZ_Model_In_InterviewRecords
            {
                UserID = p.UserID,
                ApplyJob = p.ApplyJob
            }).ToList();
        }

        /// <summary>
        /// 获取本月应聘人数
        /// </summary>
        /// <returns></returns>
        public int GetThisMonthInterviewNumber()
        {
            return _InterviewRecords.Where(p => p.AddTime.Month >= DateTime.Now.Month).Select(p => p.UserID).ToList().GroupBy(p => p).Count();
        }

        /// <summary>
        /// 根据用户ID统计用户面试次数
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public List<(int, int)> GetInterviewTimesByUIDS(List<int> userIds)
        {
            return _InterviewRecords.Where(p => userIds.Contains(p.UserID)).GroupBy(p => p.UserID).Select(p => ValueTuple.Create(p.Key, p.Count())).ToList();
        }

        /// <summary>
        /// 通过用户ID获取用户最新一条面试记录
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public QZ_Model_In_InterviewRecords GetNewInterviewInfoByUID(int uid)
        {
            return _InterviewRecords.Where(p => p.UserID == uid).OrderByDescending(p => p.ID).FirstOrDefault();
        }
        #endregion
    }
}
