using QZ.Model.Expand;
using QZ.Model.Interview;
using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Interface.Interview_IService
{
    public interface QZ_In_IUserBasicInfoService : IBaseService
    {
        #region 读取
        /// <summary>
        /// 根据用户ID获取基本信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        QZ_Model_In_UserBasicInfo GetBasicInfo(int uid);
        #endregion

        #region 写入
        /// <summary>
        /// 提交用户基本信息
        /// </summary>
        /// <param name="info">带保存的基本信息</param>
        /// <param name="basicInfo">返回基本信息</param>
        /// <returns></returns>
        bool SubmitBasicInfo(Interview_UserBasicInfo info, out QZ_Model_In_UserBasicInfo basicInfo);
        #endregion
    }
}
