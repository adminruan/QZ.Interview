using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Model.Expand.Interview
{
    /// <summary>
    /// 面试点评
    /// </summary>
    public class Interview_InterviewerRemark
    {
        /// <summary>
        /// 面试管理员ID
        /// </summary>
        public int AdminID { get; set; }

        /// <summary>
        /// 面试管理员真实姓名
        /// </summary>
        public string AdminRealName { get; set; }

        /// <summary>
        /// 评语
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 增添时间
        /// </summary>
        public DateTime AddTime { get; set; }
    }
}
