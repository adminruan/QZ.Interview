using Microsoft.EntityFrameworkCore;
using QZ.Common;
using QZ.Interface.Interview_IService;
using QZ.Model.DBContext;
using QZ.Model.Interview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QZ.Service.Interview_Service
{
    public class QZ_In_UserService : BaseService, QZ_In_IUserService
    {
        private readonly DbSet<QZ_Model_In_User> _Users;
        public QZ_In_UserService(Interview_DB_EFContext dbContext) : base(dbContext)
        {
            this._Users = dbContext.Users;
        }

        #region 读取
        /// <summary>
        /// 获取用户令牌
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public string GetUserToken(QZ_Model_In_User userInfo)
        {
            return QZ_Helper_URLUtils.UrlEncode(QZ_Helper_Encryption.DES_Encode(QZ_Helper_Encryption.Get32MD5String(userInfo.UnionID + userInfo.UserID), QZ_Helper_Encryption.Get32MD5String(userInfo.AddTime.ToString())));
        }

        /// <summary>
        /// 获取用户信息根据unionID
        /// </summary>
        /// <param name="unionID"></param>
        /// <returns></returns>
        public QZ_Model_In_User GetUserByUnionID(string unionID)
        {
            return _Users.FirstOrDefault(p => p.UnionID == unionID);
        }

        /// <summary>
        /// 获取用户信息根据openID
        /// </summary>
        /// <param name="openID"></param>
        /// <returns></returns>
        public QZ_Model_In_User GetUserByOpenID(string openID)
        {
            return _Users.FirstOrDefault(p => p.OpenID == openID);
        }
        #endregion
    }
}
