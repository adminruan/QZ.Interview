using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QZ.Common.Enums
{
    /// <summary>
    /// 职务枚举
    /// </summary>
    public enum QZ_Enum_Positions
    {
        /// <summary>
        /// 编辑
        /// </summary>
        [Description("编辑")]
        Editor = 1,

        /// <summary>
        /// 客服
        /// </summary>
        [Description("客服")]
        Service = 2,

        /// <summary>
        /// 行政
        /// </summary>
        [Description("行政")]
        Administrative = 3,

        /// <summary>
        /// .NET
        /// </summary>
        [Description(".NET")]
        DotNet = 4,

        /// <summary>
        /// Web前端
        /// </summary>
        [Description("Web前端")]
        WebFrontEnd = 5,

        /// <summary>
        /// Web前端
        /// </summary>
        [Description("网页设计")]
        Stylist = 6,

        /// <summary>
        /// 软件测试
        /// </summary>
        [Description("软件测试")]
        TestEngineer = 7,

        /// <summary>
        /// 产品经理
        /// </summary>
        [Description("产品经理")]
        Product = 8,

        /// <summary>
        /// 总经理
        /// </summary>
        [Description("总经理")]
        Boss = 9
    }
}
