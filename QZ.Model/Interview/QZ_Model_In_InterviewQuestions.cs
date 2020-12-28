using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QZ.Model.Interview
{
    /// <summary>
    /// 面试题实体
    /// </summary>
    [Table("In_InterviewQuestions")]
    public class QZ_Model_In_InterviewQuestions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [DefaultValue(0)] public double? tmfs { get; set; }

        [DefaultValue(0)] public int? tmtype { get; set; }

        [StringLength(50)]
        public string sttx { get; set; }

        [StringLength(4000)]
        public string title { get; set; }

        [Column(TypeName = "ntext")]
        public string tmnr { get; set; }

        [Column(TypeName = "ntext")]
        public string Answer { get; set; }

        [StringLength(255)]
        public string keywords { get; set; }

        [Column(TypeName = "ntext")]
        public string DAJS { get; set; }

        [Column(TypeName = "ntext")]
        public string beizhu { get; set; }

        [DefaultValue(0)] public Byte? IsMedia { get; set; }

        [StringLength(250)]
        public string MediaUrl { get; set; }

        [Column(TypeName = "ntext")]
        public string MediaTxt { get; set; }

        [DefaultValue(0)] public int? anliid { get; set; }

        [Column(TypeName = "ntext")]
        public string anlitxt { get; set; }

        /// <summary>
        /// 0 未审核题不显示不能做题   1 正常题可显示可练习    
        /// </summary>
        [DefaultValue(0)] public Byte? verific { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? adddate { get; set; }

        [StringLength(50)]
        public string user { get; set; }

        [StringLength(10)]
        public string stdj { get; set; }

        [StringLength(50)]
        public string starea { get; set; }

        [StringLength(50)]
        public string username { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? updatetime { get; set; }

        [StringLength(50)]
        public string guid { get; set; }

    }
}