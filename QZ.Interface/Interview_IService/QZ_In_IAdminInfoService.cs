using QZ.Model.Interview;
using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Interface.Interview_IService
{
    public interface QZ_In_IAdminInfoService : IBaseService
    {
        #region 读取
        /// <summary>
        /// 根据用户名或手机号码获取用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        QZ_Model_In_AdminInfo GetUserByUserNameOrMobile(string userName);

        /// <summary>
        /// 获取用户令牌
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        string GetAdminUserToken(QZ_Model_In_AdminInfo adminInfo);

        /// <summary>
        /// 获取管理员列表
        /// </summary>
        /// <returns></returns>
        List<QZ_Model_In_AdminInfo> GetAdminList();

        /// <summary>
        /// 通过AdminID获取管理员
        /// </summary>
        /// <param name="adminID"></param>
        /// <returns></returns>
        QZ_Model_In_AdminInfo GEtUserInfoByAdminID(int adminID);
        #endregion

        #region 写入
        /// <summary>
        /// 绑定公众号中openid
        /// </summary>
        /// <param name="uid">管理员ID</param>
        /// <param name="openID">在公众号中的openid</param>
        /// <returns></returns>
        bool BindOfficialOpenID(int uid, string openID);
        #endregion
    }
}
