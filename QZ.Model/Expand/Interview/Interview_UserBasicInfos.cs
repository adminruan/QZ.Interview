using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QZ.Model.Expand
{
    /// <summary>
    /// 基本信息
    /// </summary>
    public class Interview_UserBasicInfo : Interview_BaseAuthority
    {
        //public int ID { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [Required(ErrorMessage = "请输入真实姓名")]
        public string RealName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Required(ErrorMessage = "请选择性别")]
        public string Gender { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [Required(ErrorMessage = "请输入民族")]
        public string Nation { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        [Required(ErrorMessage = "请输入文化程度")]
        public string Education { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        [StringLength(18, ErrorMessage = "身份证号码有误")]
        public string IdentityNumber { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Required(ErrorMessage = "请输入出生年月")]
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
        [Required(ErrorMessage = "请输入联系方式")]
        [StringLength(11, ErrorMessage = "请填写真确的手机号码")]
        public string Moblie { get; set; }

        /// <summary>
        /// 微信号
        /// </summary>
        public string WechatID { get; set; }

        /// <summary>
        /// 紧急联系人
        /// </summary>
        [Required(ErrorMessage = "请输入紧急联系人姓名")]
        public string EmergencyContact { get; set; }

        /// <summary>
        /// 紧急联系电话
        /// </summary>
        [Required(ErrorMessage = "请输入紧急联系号码")]
        [StringLength(11, ErrorMessage = "请填写真确的手机号码")]
        public string EmergencyMobile { get; set; }

        /// <summary>
        /// 与紧急联系人的关系
        /// </summary>
        [Required(ErrorMessage = "请选择紧急联系关系")]
        public string Relation { get; set; }

        /// <summary>
        /// 申请的职位
        /// </summary>
        [Required(ErrorMessage = "请选择应聘岗位")]
        public int ApplyJob { get; set; }

        /// <summary>
        /// 期望薪资
        /// </summary>
        [Required(ErrorMessage = "请输入期望薪资")]
        [Range(1, 999999, ErrorMessage = "请真确填写薪资")]
        public decimal ExpectSalary { get; set; }

        /// <summary>
        /// 可接受薪资
        /// </summary>
        [Required(ErrorMessage = "请输入可接受薪资")]
        [Range(1, 999999, ErrorMessage = "请真确填写薪资")]
        public decimal LowSalary { get; set; }

        /// <summary>
        /// 到岗时间
        /// </summary>
        [Required(ErrorMessage = "请输入到岗时间")]
        public DateTime ArriveTime { get; set; }

        /// <summary>
        /// 简历来源平台
        /// </summary>
        [Required(ErrorMessage = "请选择简历来源")]
        public int ResumeSource { get; set; }

        /// <summary>
        /// 教育经历
        /// </summary>
        public List<Interview_UserEducation> Educations { get; set; }

        /// <summary>
        /// 教育经历json字符
        /// </summary>
        public string EducationsJson { get; set; }

        /// <summary>
        /// 工作经历
        /// </summary>
        public List<Interview_UserHistoryJob> Jobs { get; set; }

        /// <summary>
        /// 工作经历json字符
        /// </summary>
        public string JobsJson { get; set; }
    }

    /// <summary>
    /// 教育经历
    /// </summary>
    public class Interview_UserEducation
    {
        /// <summary>
        /// 教育类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        public string School { get; set; }

        /// <summary>
        /// 入学时间
        /// </summary>
        public DateTime AdmissionTime { get; set; }

        /// <summary>
        /// 毕业时间
        /// </summary>
        public DateTime GraduationTime { get; set; }

        /// <summary>
        /// 专业
        /// </summary>
        public string Major { get; set; }
    }

    /// <summary>
    /// 工作经历
    /// </summary>
    public class Interview_UserHistoryJob
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
