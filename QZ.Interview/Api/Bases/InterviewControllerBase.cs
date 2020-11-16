using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using QZ.Common;
using QZ.Interface.Interview_IService;
using QZ.Model.Interview;

namespace QZ.Interview.Api.Bases
{
    [InterviewFilter]
    public class InterviewControllerBase : Controller
    {
        public InterviewControllerBase(QZ_In_IUserService iUserService)
        {
            this._iBaseUserService = iUserService;
        }
        public InterviewControllerBase(QZ_In_IAdminInfoService iAdminInfoService)
        {
            this._iBaseAdminInfoService = iAdminInfoService;
        }

        protected readonly QZ_In_IUserService _iBaseUserService;
        protected readonly QZ_In_IAdminInfoService _iBaseAdminInfoService;

        /// <summary>
        /// 时间格式
        /// </summary>
        private string DateFomart = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 指定特性
        /// 格式：A|B|C
        /// </summary>
        private string AppointPropertys = "";
        /// <summary>
        /// 指定特性状态
        /// True：只响应指定的特性
        /// Flase：不响应指定的特性
        /// </summary>
        private bool APResponse = true;

        /// <summary>
        /// 响应码枚举
        /// </summary>
        protected enum EnumResponseCode
        {
            /// <summary>
            /// 未登录
            /// </summary>
            [Description("未登录")]
            NotSignIn = -1,

            /// <summary>
            /// 错误请求
            /// </summary>
            [Description("错误请求")]
            Error = 0,

            /// <summary>
            /// 成功
            /// </summary>
            [Description("成功")]
            Success = 1
        }

        #region 响应处理
        /// <summary>
        /// 字典类型响应
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected JsonResult Write(EnumResponseCode code, string message = "", Dictionary<string, string> data = null)
        {
            Dictionary<string, object> pairs = new Dictionary<string, object>();
            pairs.Add("SIP", QZ_Helper_Encryption.Base64Encode(QZ_Helper_IP.GetServiceIP()));
            pairs.Add("Date", QZ_Helper_Encryption.Base64Encode(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            pairs.Add("S", QZ_Helper_Encryption.Base64Encode(((int)code).ToString()));
            if (string.IsNullOrWhiteSpace(message))
                message = code.GetEnumDescription();
            pairs.Add("msg", QZ_Helper_Encryption.Base64Encode(message));
            if (data != null && data.Count > 0)
            {
                pairs.Add("data", ProcessingDictionary(data));
            }
            return Json(pairs);
        }

        /// <summary>
        /// 对象类型响应
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">响应对象</param>
        /// <param name="appoints">指定的属性</param>
        /// <param name="appointResponse">指定响应状态（true：指定的属性响应、false：指定的不响应）</param>
        /// <returns></returns>
        [NonAction]
        protected JsonResult Write<T>(T t, string appoints = "", bool appointResponse = true) where T : class
        {
            Dictionary<string, object> pairs = new Dictionary<string, object>();
            pairs.Add("SIP", QZ_Helper_Encryption.Base64Encode(QZ_Helper_IP.GetServiceIP()));
            pairs.Add("Date", QZ_Helper_Encryption.Base64Encode(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            pairs.Add("S", QZ_Helper_Encryption.Base64Encode(((int)EnumResponseCode.Success).ToString()));
            pairs.Add("msg", QZ_Helper_Encryption.Base64Encode(EnumResponseCode.Success.GetEnumDescription()));
            if (t != null)
            {
                if (!string.IsNullOrWhiteSpace(appoints))
                {
                    AppointPropertys = appoints;
                    APResponse = appointResponse;
                }
                pairs.Add("data", ProcessingObject(t));
            }
            return Json(pairs);
        }

        /// <summary>
        /// list类型响应
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">list数据</param>
        /// <param name="appoints">指定的字段多个用“|”隔开</param>
        /// <param name="appointRes">True：指定的字段响应、False：指定的字段隐藏</param>
        /// <param name="message">提示ixnxi</param>
        /// <param name="dateFomart">时间格式</param>
        /// <param name="data">附带的data数据</param>
        /// <returns></returns>
        protected JsonResult Writes<T>(List<T> list, string appoints = "", bool appointRes = true, string message = "", string dateFomart = "", Dictionary<string, string> data = null) where T : class
        {
            if (!string.IsNullOrWhiteSpace(dateFomart))
            {
                DateFomart = dateFomart;
            }
            Dictionary<string, object> pairs = new Dictionary<string, object>();
            pairs.Add("SIP", QZ_Helper_Encryption.Base64Encode(QZ_Helper_IP.GetServiceIP()));
            pairs.Add("Date", QZ_Helper_Encryption.Base64Encode(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            pairs.Add("S", QZ_Helper_Encryption.Base64Encode(((int)EnumResponseCode.Success).ToString()));
            pairs.Add("msg", QZ_Helper_Encryption.Base64Encode(string.IsNullOrEmpty(message) ? EnumResponseCode.Success.GetEnumDescription() : message));
            if (data != null && data.Count > 0)
            {
                pairs.Add("data", ProcessingDictionary(data));
            }
            List<Dictionary<string, string>> listPairs = new List<Dictionary<string, string>>();
            foreach (var item in list)
            {
                Type type = item.GetType();
                PropertyInfo[] properties = type.GetProperties();
                if (!string.IsNullOrWhiteSpace(appoints))
                {
                    //指定字段响应或隐藏
                    List<string> appointList = appointList = appoints.Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (appointRes)
                    {
                        properties = properties.Where(p => appointList.Contains(p.Name)).ToArray();
                    }
                    else
                    {
                        properties = properties.Where(p => !appointList.Contains(p.Name)).ToArray();
                    }
                }
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                foreach (var property in properties)
                {
                    KeyValuePair<string, string> keyValue = PropertyDispose(property, item);
                    if (!keyValue.Equals(default(KeyValuePair<string, string>)))
                    {
                        valuePairs.Add(keyValue.Key, keyValue.Value);
                    }
                }
                listPairs.Add(valuePairs);
            }
            pairs.Add("datas", listPairs);
            return Json(pairs);
        }

        /// <summary>
        /// 字典值处理
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private Dictionary<string, string> ProcessingDictionary(Dictionary<string, string> datas)
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            foreach (var item in datas)
            {
                pairs.Add(item.Key, QZ_Helper_Encryption.Base64Encode(item.Value));
            }
            return pairs;
        }

        /// <summary>
        /// 对象值处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        private Dictionary<string, object> ProcessingObject<T>(T t) where T : class
        {
            Dictionary<string, object> pairs = new Dictionary<string, object>();
            Type type = t.GetType();
            PropertyInfo[] properties = type.GetProperties();
            //响应字段处理
            if (!string.IsNullOrWhiteSpace(AppointPropertys))
            {
                List<string> assigns = new List<string>();
                if (AppointPropertys.Contains("|"))
                {
                    assigns = AppointPropertys.Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    assigns.Add(AppointPropertys);
                }
                if (APResponse)
                {
                    //指定的响应
                    properties = properties.Where(p => assigns.Contains(p.Name)).ToArray();
                }
                else
                {
                    //指定的不响应
                    properties = properties.Where(p => !assigns.Contains(p.Name)).ToArray();
                }
            }
            foreach (var item in properties)
            {
                string name = item.Name;
                //格式处理
                if (name.ToUpper() == name)
                {
                    name = name.ToLower();
                }
                else
                {
                    string nameF = name.Substring(0, 1).ToLower();
                    string nameO = name.Substring(1, name.Length - 1);
                    name = nameF + nameO;
                }
                if (item.GetValue(t) == null)
                {
                    pairs.Add(name, "");
                }
                else if (item.PropertyType.Name == "Boolean")
                {
                    pairs.Add(name, QZ_Helper_Encryption.Base64Encode(item.GetValue(t).ToString().ToLower()));
                }
                else if (item.PropertyType.Name == "DateTime")
                {
                    pairs.Add(name, QZ_Helper_Encryption.Base64Encode(Convert.ToDateTime(item.GetValue(t)).ToString(DateFomart)));
                }
                else if (item.PropertyType.Name == "Decimal")
                {
                    pairs.Add(name, QZ_Helper_Encryption.Base64Encode(Convert.ToDecimal(item.GetValue(t)).ToString("0.00")));
                }
                else if (item.PropertyType.Name == "Object")
                {
                    pairs.Add(name, ProcessingObject(item.GetValue(t)));
                }
                else if (item.PropertyType.Name == "List`1")
                {
                    object itemValues = item.GetValue(t);
                    if (itemValues is List<QZ.Model.Expand.Interview_UserEducation>)
                    {
                        var testModel = itemValues as List<dynamic>;
                        var testModel2 = itemValues as List<object>;
                        pairs.Add(name, ProcessingList((List<QZ.Model.Expand.Interview_UserEducation>)itemValues));
                    }
                    else if (itemValues is List<QZ.Model.Expand.Interview_UserHistoryJob>)
                    {
                        pairs.Add(name, ProcessingList((List<QZ.Model.Expand.Interview_UserHistoryJob>)itemValues));
                    }
                    else if (itemValues is List<QZ.Model.Expand.Interview.Interview_InterviewerRemark>)
                    {
                        pairs.Add(name, ProcessingList((List<QZ.Model.Expand.Interview.Interview_InterviewerRemark>)itemValues));
                    }
                }
                else
                {
                    pairs.Add(name, QZ_Helper_Encryption.Base64Encode(item.GetValue(t).ToString()));
                }
            }
            return pairs;
        }

        private List<Dictionary<string, string>> ProcessingList<T>(List<T> list) where T : class
        {
            List<Dictionary<string, string>> listPairs = new List<Dictionary<string, string>>();
            foreach (var item in list)
            {
                Type type = item.GetType();
                PropertyInfo[] properties = type.GetProperties();
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                foreach (var property in properties)
                {
                    KeyValuePair<string, string> keyValue = PropertyDispose(property, item);
                    if (!keyValue.Equals(default(KeyValuePair<string, string>)))
                    {
                        valuePairs.Add(keyValue.Key, keyValue.Value);
                    }
                }
                listPairs.Add(valuePairs);
            }
            return listPairs;
        }

        /// <summary>
        /// 成员属性处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">属性</param>
        /// <param name="t">属性来源</param>
        /// <param name="dateType">时间类型</param>
        /// <returns></returns>
        private KeyValuePair<string, string> PropertyDispose<T>(PropertyInfo property, T t) where T : class
        {
            KeyValuePair<string, string> pair = default(KeyValuePair<string, string>);
            try
            {
                string name = property.Name;
                //转换成小驼峰格式
                if (name.ToUpper() == name)
                {
                    name = name.ToLower();
                }
                else
                {
                    string nameF = name.Substring(0, 1).ToLower();
                    string nameO = name.Substring(1, name.Length - 1);
                    name = nameF + nameO;
                }
                if (string.IsNullOrWhiteSpace(property.GetValue(t).ToString()))
                {
                    pair = new KeyValuePair<string, string>(name, "");
                }
                //处理bool类型转换为字符串时大写问题
                else if (property.PropertyType.Name == "Boolean")
                {
                    pair = new KeyValuePair<string, string>(name, QZ_Helper_Encryption.Base64Encode(property.GetValue(t).ToString().ToLower()));
                }
                //处理时间格式
                else if (property.PropertyType.Name == "DateTime")
                {
                    try
                    {
                        pair = new KeyValuePair<string, string>(name, QZ_Helper_Encryption.Base64Encode(Convert.ToDateTime(property.GetValue(t)).ToString(DateFomart)));
                    }
                    catch (Exception)
                    {
                        pair = new KeyValuePair<string, string>(name, QZ_Helper_Encryption.Base64Encode(Convert.ToDateTime(property.GetValue(t)).ToString("yyyy-MM-dd HH:mm:ss")));
                    }
                }
                //处理金额格式
                else if (property.PropertyType.Name == "Decimal")
                {
                    pair = new KeyValuePair<string, string>(name, QZ_Helper_Encryption.Base64Encode(Convert.ToDecimal(property.GetValue(t)).ToString("0.00")));
                }
                else
                {
                    try
                    {
                        pair = new KeyValuePair<string, string>(name, QZ_Helper_Encryption.Base64Encode(property.GetValue(t).ToString()));
                    }
                    catch (Exception)
                    {
                        pair = new KeyValuePair<string, string>(name, "");
                    }
                }
                return pair;
            }
            catch (Exception)
            {
                return pair;
            }
        }
        #endregion

        #region 登录校验
        /// <summary>
        /// 普通用户登录校验
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="userToken">用户令牌</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns></returns>
        [NonAction]
        protected bool ValidUser(int userID, string userToken, out QZ_Model_In_User userInfo)
        {
            if (userID <= 0 || string.IsNullOrWhiteSpace(userToken))
            {
                userInfo = new QZ_Model_In_User();
                return false;
            }

            QZ_Model_In_User userModel = this._iBaseUserService.Find<QZ_Model_In_User>(userID);
            if (userModel != null)
            {
                string newToken = QZ_Helper_URLUtils.UrlDecode(this._iBaseUserService.GetUserToken(userModel));
                if (userToken.Contains("%"))
                {
                    userToken = QZ_Helper_URLUtils.UrlDecode(userToken);
                }
                if (userToken.Replace(" ", "+") == newToken)
                {
                    userInfo = userModel;
                    return true;
                }
            }
            userInfo = new QZ_Model_In_User();
            return false;
        }

        /// <summary>
        /// 管理员用户登录校验
        /// </summary>
        /// <param name="adminID">管理员ID</param>
        /// <param name="aminToken">管理员令牌</param>
        /// <param name="adminInfo">管理员信息</param>
        /// <returns></returns>
        [NonAction]
        protected bool ValidAdminUser(int adminID, string aminToken, out QZ_Model_In_AdminInfo adminInfo)
        {
            if (adminID <= 0 || string.IsNullOrWhiteSpace(aminToken))
            {
                adminInfo = new QZ_Model_In_AdminInfo();
                return false;
            }

            QZ_Model_In_AdminInfo userModel = this._iBaseAdminInfoService.Find<QZ_Model_In_AdminInfo>(adminID);
            if (userModel != null)
            {
                string newToken = QZ_Helper_URLUtils.UrlDecode(this._iBaseAdminInfoService.GetAdminUserToken(userModel));
                if (aminToken.Contains("%"))
                {
                    aminToken = QZ_Helper_URLUtils.UrlDecode(aminToken);
                }
                if (aminToken.Replace(" ", "+") == newToken)
                {
                    adminInfo = userModel;
                    return true;
                }
            }
            adminInfo = new QZ_Model_In_AdminInfo();
            return false;
        }
        #endregion
    }
}
