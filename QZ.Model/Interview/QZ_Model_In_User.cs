using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QZ.Model.Interview
{
    /// <summary>
    /// 用户信息实体
    /// </summary>
    [Table("In_User")]
    public class QZ_Model_In_User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [StringLength(125)]
        public string NickName { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [StringLength(325)]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// OpenID
        /// </summary>
        [Required]
        [StringLength(50)]
        public string OpenID { get; set; }


        /// <summary>
        /// UnionID
        /// </summary>
        [Required]
        [StringLength(100)]
        public string UnionID { get; set; }

        /// <summary>
        /// 性别
        /// True=男
        /// </summary>
        public bool? Gender { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [StringLength(11)]
        public string Mobile { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        [StringLength(20)]
        public string Country { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        [StringLength(20)]
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [StringLength(20)]
        public string City { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime AddTime { get; set; }
    }
}
