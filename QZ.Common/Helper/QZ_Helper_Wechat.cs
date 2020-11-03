using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QZ.Model.Expand.Wechat;
using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Common
{
    public static class QZ_Helper_Wechat
    {
        private readonly static string _URL = "https://api.weixin.qq.com/";

        #region 获取 SessionKey
        /// <summary>
        /// 获取微信session key
        /// </summary>
        /// <param name="appid">appid</param>
        /// <param name="secret">通讯秘钥</param>
        /// <param name="js_code">微信一次性凭证</param>
        /// <returns></returns>
        public static string GetAppSessionKey(string appid, string secret, string js_code)
        {
            //发送并接受返回值   
            string send_url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + appid + "&secret=" + secret + "&js_code=" + js_code + "&grant_type=authorization_code";
            return QZ_Helper_HttpMethods.HttpGet(send_url);
        }
        #endregion

        #region 获取 AccessToken 凭证
        /// <summary>
        /// 获取微信全局唯一接口调用凭据
        /// </summary>
        /// <param name="appID">唯一凭证</param>
        /// <param name="appSerect">凭证密钥</param>
        /// <param name="accessToken">用户令牌</param>
        /// <returns></returns>
        public static bool GetAccessToken(out string accessToken, string appID = "", string appSerect = "")
        {
            try
            {
                string path = $"{_URL}cgi-bin/token?grant_type=client_credential&appid={appID}&secret={appSerect}";
                string responseStr = QZ_Helper_HttpMethods.HttpGet(path);
                if (responseStr.Contains("access_token"))
                {
                    //成功
                    WeChat_Model_AccessToken accessTokenInfo = new WeChat_Model_AccessToken();
                    accessTokenInfo = JsonConvert.DeserializeObject<WeChat_Model_AccessToken>(responseStr);
                    accessToken = accessTokenInfo.AccessToken;
                    if (string.IsNullOrWhiteSpace(accessToken))
                    {
                        accessToken = responseStr;
                        return true;
                    }
                }
                else if (responseStr.Contains("40164"))
                {
                    //IP未加入到白名单
                    JObject jsonResult = JsonConvert.DeserializeObject<JObject>(responseStr);
                    accessToken = jsonResult.Value<string>("errmsg");
                    return false;
                }
                else
                {
                    //未知错误
                    JObject jsonResult = JsonConvert.DeserializeObject<JObject>(responseStr);
                    accessToken = jsonResult.Value<string>("errmsg");
                    return false;
                }
                accessToken = responseStr;
                return false;
            }
            catch (Exception e)
            {
                accessToken = e.Message;
                return false;
            }
        }
        #endregion

        #region 发送公众号模板消息
        /// <summary>
        /// 发送公众号模板消息
        /// </summary>
        /// <param name="accessToken">AccessToken</param>
        /// <param name="jsonData">待发送的json格式数据</param>
        /// <returns></returns>
        public static bool SendTemplateMessage(string accessToken, string jsonData)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={accessToken}";
            string responseStr = QZ_Helper_HttpMethods.HttpPost(url, jsonData);
            if (string.IsNullOrWhiteSpace(responseStr) || !responseStr.Contains("ok"))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 公众号消息模板
        /// <summary>
        /// 公众号消息模板
        /// </summary>
        /// <param name="openId">接受消息用户OpenID</param>
        /// <param name="templateId">消息模板ID</param>
        /// <param name="appID">点击模板消息需要跳转的小程序</param>
        /// <param name="pagePath">所需跳转到小程序的具体页面路径，支持带参数,（示例index?foo=bar）</param>
        /// <param name="type">1：待处理面试通知模板、2：通知面试官面试模板</param>
        /// <returns></returns>
        public static string AwaitInterviewTemplate(string openId, string templateId = "nuoUIwDpMb1T_MrcU1WkG0UV-HclVR0Jl7lWBdf4-Tg", string appID = "", string pagePath = "", int type = 1)
        {
            Dictionary<string, dynamic> parameter = new Dictionary<string, dynamic>();
            parameter.Add("touser", openId);
            if (!string.IsNullOrWhiteSpace(appID))
            {
                Dictionary<string, string> miniProgramData = new Dictionary<string, string>();
                miniProgramData.Add("appid", appID);
                if (!string.IsNullOrWhiteSpace(pagePath))
                    miniProgramData.Add("pagepath", pagePath);
                parameter.Add("miniprogram", miniProgramData);
            }
            parameter.Add("template_id", templateId);//消息模板ID

            switch (type)
            {
                case 2:
                    //通知面试官面试模板
                    parameter.Add("data", "这里是待发送的数据信息");
                    break;
                default:
                    //待处理面试通知模板
                    parameter.Add("data", "这里是待发送的数据信息");
                    break;
            }
            return JsonConvert.SerializeObject(parameter);
        }
        #endregion
    }
}
