using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QZ.Common.Enums
{
    /// <summary>
    /// 面试进度状态枚举
    /// </summary>
    public enum QZ_Enum_Schedules
    {
        /// <summary>
        /// 待分配
        /// </summary>
        [Description("待分配")]
        AwaitPlan = 100,

        /// <summary>
        /// 一面
        /// </summary>
        [Description("一面")]
        InterviewFirst = 200,

        /// <summary>
        /// 二面
        /// </summary>
        [Description("二面")]
        InterviewSencond = 201,

        /// <summary>
        /// 入职审批
        /// </summary>
        [Description("入职审批")]
        PendingApproval = 300,

        /// <summary>
        /// 不合适
        /// </summary>
        [Description("不合适")]
        Fail = 301,

        /// <summary>
        /// 备用
        /// </summary>
        [Description("备用")]
        Spare = 302,

        /// <summary>
        /// 通过
        /// </summary>
        [Description("通过")]
        Qualified = 303
    }
}
