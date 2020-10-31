using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Model.Expand
{
    /// <summary>
    /// 响应基类
    /// </summary>
    public class Response_BaseModel
    {
        public string SIP { get; set; }
        public string Date { get; set; }
        public string S { get; set; }
        public string msg { get; set; }
    }
}
