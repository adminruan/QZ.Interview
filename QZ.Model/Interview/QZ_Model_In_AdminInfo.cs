using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QZ.Model.Interview
{
    [Table("In_AdminInfo")]
    public class QZ_Model_In_AdminInfo
    {
        [Key]
        public int AdminID { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [Required]
        [StringLength(10, ErrorMessage = "姓名最大长度为10位.")]
        public string RealName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [StringLength(11, ErrorMessage = "手机号码最大长度为11位.")]
        public string Mobile { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        [Required]
        public int Position { get; set; }

        /// <summary>
        /// 状态
        /// 0：正常
        /// 1：已锁定
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 增添时间    
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 公众号中OpenID
        /// 用于发送微信公众号通知
        /// </summary>
        public string OpenID { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        #region 辅助字段
        /// <summary>
        /// 职务名称
        /// </summary>
        [NotMapped]
        public string ExtPositionName { get; set; }
        #endregion
    }
}
