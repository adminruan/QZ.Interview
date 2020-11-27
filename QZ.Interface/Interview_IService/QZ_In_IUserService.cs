using QZ.Model.Interview;
using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Interface.Interview_IService
{
    public interface QZ_In_IUserService : IBaseService
    {
        #region 读取
        /// <summary>
        /// 获取用户令牌
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        string GetUserToken(QZ_Model_In_User userInfo);

        /// <summary>
        /// 获取用户信息根据unionID
        /// </summary>
        /// <param name="unionID"></param>
        /// <returns></returns>
        QZ_Model_In_User GetUserByUnionID(string unionID);

        /// <summary>
        /// 获取用户信息根据openID
        /// </summary>
        /// <param name="openID"></param>
        /// <returns></returns>
        QZ_Model_In_User GetUserByOpenID(string openID);
        #endregion

        #region 写入
        /// <summary>
        /// 绑定接受消息通知公众号OpenID
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="offOpenID"></param>
        /// <returns></returns>
        bool BindOfficialOpenID(int uid, string offOpenID);
        #endregion
    }
}
