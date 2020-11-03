using QZ.Model.Interview;
using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Interface.Interview_IService
{
    public interface QZ_In_IPositionsService : IBaseService
    {
        #region 读取
        /// <summary>
        /// 获取职位
        /// </summary>
        /// <returns></returns>
        List<QZ_Model_In_Positions> GetPositions();
        #endregion
    }
}
