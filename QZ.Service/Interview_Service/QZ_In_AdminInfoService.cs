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
    public class QZ_In_AdminInfoService : BaseService, QZ_In_IAdminInfoService
    {
        private readonly DbSet<QZ_Model_In_AdminInfo> _AdminInfos;
        public QZ_In_AdminInfoService(Interview_DB_EFContext dbContext) : base(dbContext)
        {
            this._AdminInfos = dbContext.AdminInfos;
        }

        #region 读取
        /// <summary>
        /// 根据用户名或手机号码获取用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public QZ_Model_In_AdminInfo GetUserByUserNameOrMobile(string userName)
        {
            return _AdminInfos.FirstOrDefault(p => p.UserName == userName || p.Mobile == userName);
        }

        /// <summary>
        /// 获取用户令牌
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public string GetAdminUserToken(QZ_Model_In_AdminInfo adminInfo)
        {
            return QZ_Helper_URLUtils.UrlEncode(QZ_Helper_Encryption.DES_Encode(QZ_Helper_Encryption.Get32MD5String(adminInfo.UserName + adminInfo.AdminID), QZ_Helper_Encryption.Get32MD5String(adminInfo.CreateTime.ToString())));
        }
        #endregion
    }
}
