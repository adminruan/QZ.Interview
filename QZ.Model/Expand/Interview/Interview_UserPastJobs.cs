using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QZ.Model.Expand.Interview
{
    public class Interview_UserPastJobs
    {
        [Required(ErrorMessage = "{0}有误")]
        public int ID { get; set; }

        [Required(ErrorMessage = "{0}有误")]
        public List<Interview_UserPastJob> UserPastJobs { get; set; }
    }

    /// <summary>
    /// 用户工作经历
    /// </summary>
    public class Interview_UserPastJob
    {
        /// <summary>
        /// 开始时间
        /// </summary>

        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 薪资
        /// </summary>
        public decimal Salary { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        public string Duty { get; set; }

        /// <summary>
        /// 离职原因
        /// </summary>
        public string Remark { get; set; }
    }
}
