using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QZ.Common.Enums
{
    /// <summary>
    /// 简历来源平台
    /// </summary>
    public enum QZ_Enum_RecruitPlatform
    {
        /// <summary>
        /// 前程无忧
        /// </summary>
        [Description("前程无忧")]
        QianChengJobs = 1,

        /// <summary>
        /// Boss直聘
        /// </summary>
        [Description("Boss直聘")]
        BossJobs = 2,

        /// <summary>
        /// 智联招聘
        /// </summary>
        [Description("智联招聘")]
        ZhiLianJobs = 3,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Others = 4
    }
}
