using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QZ.Common.Enums
{
    /// <summary>
    /// 教育类型
    /// </summary>
    public enum QZ_Enum_EducationTypes
    {
        /// <summary>
        /// 统招
        /// </summary>
        [Description("统招")]
        Recruitment = 1,

        /// <summary>
        /// 自考
        /// </summary>
        [Description("自考")]
        SelfStudyExamaination = 2,

        /// <summary>
        /// 函授
        /// </summary>
        [Description("函授")]
        TeachByCorrespondence = 3,

        /// <summary>
        /// 培训
        /// </summary>
        [Description("培训")]
        Train = 4
    }
}
