using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QZ.Model.Expand.Interview
{
    public class Interview_UserEducationsNew
    {
        [Required(ErrorMessage = "{0}有误")]
        public int ID { get; set; }

        [Required(ErrorMessage = "{0}有误")]
        public List<Interview_UserEducation> Educations { get; set; }
    }
    /// <summary>
    /// 用户教育经历
    /// </summary>
    public class Interview_UserEducation
    {
        /// <summary>
        /// 教育类型
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public string Type { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public string School { get; set; }

        /// <summary>
        /// 入学时间
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public DateTime AdmissionTime { get; set; }

        /// <summary>
        /// 毕业时间
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public DateTime GraduationTime { get; set; }

        /// <summary>
        /// 专业
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public string Major { get; set; }
    }
}
