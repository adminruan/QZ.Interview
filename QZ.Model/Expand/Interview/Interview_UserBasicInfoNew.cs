using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QZ.Model.Expand.Interview
{
    /// <summary>
    /// 新版用户基本信息
    /// </summary>
    public class Interview_UserBasicInfoNew
    {
        /// <summary>
        /// 用户记录ID
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public int ID { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public string RealName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public string Gender { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public string Nation { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public string Education { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        [StringLength(18, ErrorMessage = "{0}有误")]
        public string IdentityNumber { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 婚否
        /// True：已婚
        /// </summary>
        public bool Marriage { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        public string NativePlace { get; set; }

        /// <summary>
        /// 户籍性质
        /// True：务农
        /// False：非农
        /// </summary>
        public bool Farmer { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        [StringLength(11, ErrorMessage = "请正确填写{0}")]
        public string Moblie { get; set; }

        /// <summary>
        /// 微信号
        /// </summary>
        public string WechatID { get; set; }

        /// <summary>
        /// 紧急联系人
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public string EmergencyContact { get; set; }

        /// <summary>
        /// 紧急联系电话
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        [StringLength(11, ErrorMessage = "请正确填写{0}")]
        public string EmergencyMobile { get; set; }

        /// <summary>
        /// 与紧急联系人的关系
        /// </summary>
        [Required(ErrorMessage = "{0}有误")]
        public string Relation { get; set; }
    }
}
