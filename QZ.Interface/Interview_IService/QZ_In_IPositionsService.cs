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

        /// <summary>
        /// 通过ID获取职位信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        QZ_Model_In_Positions GetInfoByID(int id);
        #endregion
    }
}
