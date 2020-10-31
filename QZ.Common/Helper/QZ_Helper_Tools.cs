using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Common.Helper
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class QZ_Helper_Tools
    {
        /// <summary>
        /// 计算总页数
        /// </summary>
        /// <param name="total">总行数</param>
        /// <param name="limit">每页行数</param>
        /// <returns></returns>
        public static int CalculateTotalPageNumber(this int total, int limit)
        {
            if (total < 1)
            {
                return 0;
            }
            int number = total / limit;
            if (total % limit > 0)
            {
                number++;
            }
            return number;
        }
    }
}
