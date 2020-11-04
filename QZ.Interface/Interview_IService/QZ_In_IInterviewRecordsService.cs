using QZ.Common.Enums;
using QZ.Model.Interview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QZ.Interface.Interview_IService
{
    public interface QZ_In_IInterviewRecordsService : IBaseService
    {
        #region 写入
        /// <summary>
        /// 提交面试记录
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="adminID">负责处理管理员ID</param>
        /// <returns></returns>
        bool SubmitInterviewRecord(int uid, int adminID);

        /// <summary>
        /// 更新下轮面试人
        /// </summary>
        /// <param name="records">面试记录</param>
        /// <param name="adminID">面试官管理员ID</param>
        /// <param name="schedules">下轮面进度</param>
        /// <returns></returns>
        bool ArrangeInterviewer(QZ_Model_In_InterviewRecords records, int adminID, QZ_Enum_Schedules schedules);
        #endregion

        #region 读取
        ///// <summary>
        ///// 获取面试信息
        ///// </summary>
        ///// <param name="page">当前页</param>
        ///// <param name="limit">当前行数</param>
        ///// <param name="adminID">管理员ID</param>
        ///// <param name="isUsed">查询类型（True：已处理、False：待处理）</param>
        ///// <param name="total">总行数</param>
        ///// <param name="schedule">查询已处理，可指定筛选状态（4：不适合、5：备用、6：适合）</param>
        ///// <returns></returns>
        //List<QZ_Model_In_UserBasicInfo> GetInterviews(int page, int limit, int adminID, bool isUsed, out int total, byte? schedule);

        /// <summary>
        /// 获取面试信息
        /// </summary>
        /// <returns></returns>
        IQueryable<QZ_Model_In_UserBasicInfo> GetDataInterview();

        /// <summary>
        /// 获取面试信息
        /// 后台管理使用
        /// </summary>
        /// <returns></returns>
        IQueryable<QZ_Model_In_UserBasicInfo> GetData();

        /// <summary>
        /// 通过用户ID获取用户面试信息
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <returns></returns>
        QZ_Model_In_UserBasicInfo GetInterviewInfoByUID(int uid);
        #endregion
    }
}
