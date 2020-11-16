using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QZ.Model.Expand.Interview
{
    /// <summary>
    /// 用户申请的职位及薪资信息
    /// </summary>
    public class Interview_UserApplyJobs
    {
        /// <summary>
        /// 用户记录
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public int ID { get; set; }

        /// <summary>
        /// 申请的职位
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public int ApplyJob { get; set; }

        /// <summary>
        /// 期望薪资
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        [Range(1, 9999999, ErrorMessage = "{0}有误")]
        public decimal ExpectSalary { get; set; }

        /// <summary>
        /// 可接受薪资
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        [Range(1, 9999999, ErrorMessage = "{0}有误")]
        public decimal LowSalary { get; set; }

        /// <summary>
        /// 到岗时间
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        [DataType(DataType.DateTime)]
        public DateTime ArriveTime { get; set; }
    }
}
