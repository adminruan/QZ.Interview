using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QZ.Model.Expand
{
    /// <summary>
    /// 授权基类
    /// </summary>
    public class Interview_BaseAuthority
    {
        [Required(ErrorMessage = "{0}无效")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "{0}无效")]
        public string UserToken { get; set; }

        public string Source { get; set; }

        public string Sign { get; set; }
    }

    /// <summary>
    /// 无需授权基类 
    /// </summary>
    public class Interview_BaseNotAuthority
    {
        public string Source { get; set; }
        public string Sign { get; set; }
    }
}
