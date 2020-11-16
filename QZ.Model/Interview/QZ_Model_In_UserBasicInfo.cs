using QZ.Model.Expand;
using QZ.Model.Expand.Interview;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QZ.Model.Interview
{
    /// <summary>
    /// 用户基本信息实体
    /// </summary>
    [Table("In_UserBasicInfo")]
    public class QZ_Model_In_UserBasicInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [StringLength(10)]
        public string RealName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [StringLength(1)]
        public string Gender { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [StringLength(10)]
        public string Nation { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        [StringLength(10)]
        public string Education { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        [StringLength(18)]
        public string IdentityNumber { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
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
        [StringLength(10)]
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
        [StringLength(11)]
        public string Moblie { get; set; }

        /// <summary>
        /// 微信号
        /// </summary>
        [StringLength(25)]
        public string WechatID { get; set; }

        /// <summary>
        /// 紧急联系人
        /// </summary>
        [StringLength(10)]
        public string EmergencyContact { get; set; }

        /// <summary>
        /// 紧急联系电话
        /// </summary>
        [StringLength(11)]
        public string EmergencyMobile { get; set; }

        /// <summary>
        /// 与紧急联系人的关系
        /// </summary>
        [StringLength(10)]
        public string Relation { get; set; }

        /// <summary>
        /// 教育经历
        /// </summary>
        public string Educations { get; set; }

        /// <summary>
        /// 工作经历
        /// </summary>
        public string Jobs { get; set; }

        /// <summary>
        /// 申请的职位
        /// </summary>
        public int ApplyJob { get; set; }

        /// <summary>
        /// 期望薪资
        /// </summary>
        public decimal ExpectSalary { get; set; }

        /// <summary>
        /// 可接受薪资
        /// </summary>
        public decimal LowSalary { get; set; }

        /// <summary>
        /// 到岗时间
        /// </summary>
        public DateTime ArriveTime { get; set; }

        /// <summary>
        /// 简历来源平台
        /// </summary>
        public int ResumeSource { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime Updatetime { get; set; }

        #region 辅助字段
        /// <summary>
        /// 教育经历
        /// </summary>
        [NotMapped]
        public List<Interview_UserEducation> ExtEducations { get; set; }

        /// <summary>
        /// 工作经历
        /// </summary>
        [NotMapped]
        public List<Interview_UserHistoryJob> ExtJobs { get; set; }

        /// <summary>
        /// 面试记录ID
        /// </summary>
        [NotMapped]
        public int ExtInterviewID { get; set; }

        /// <summary>
        /// 面试申请提交时间
        /// </summary>
        [NotMapped]
        public DateTime ExtInterviewDate { get; set; }

        /// <summary>
        /// 面试进度（状态）
        /// </summary>
        [NotMapped]
        public int ExtSchedule { get; set; }

        /// <summary>
        ///  面试进度（状态）
        /// </summary>
        [NotMapped]
        public string ExtScheduleText { get; set; }

        /// <summary>
        /// 负责操作的管理员ID
        /// </summary>
        [NotMapped]
        public string ExtAdminIds { get; set; }

        /// <summary>
        /// 面试评语
        /// </summary>
        [NotMapped]
        public string ExtRemarks { get; set; }

        /// <summary>
        /// 面试评语
        /// </summary>
        [NotMapped]
        public List<Interview_InterviewerRemark> ExtRemarkList { get; set; }

        /// <summary>
        /// 历史面试记录
        /// </summary>
        [NotMapped]
        public List<QZ_Model_In_UserBasicInfo> ExtHistoryInterviews { get; set; }

        /// <summary>
        /// 简历来源平台
        /// </summary>
        [NotMapped]
        public string ExtResumeSource { get; set; }

        /// <summary>
        /// 预计到岗日期
        /// </summary>
        [NotMapped]
        public DateTime? ExtArriveTime { get; set; }

        /// <summary>
        /// 申请的岗位
        /// </summary>
        [NotMapped]
        public string ExtApplyJob { get; set; }

        /// <summary>
        /// 一面时间
        /// </summary>
        [NotMapped]
        public DateTime? ExtFirstDate { get; set; }

        /// <summary>
        /// 二面时间
        /// </summary>
        [NotMapped]
        public DateTime? ExtSecondDate { get; set; }
        #endregion
    }
}
