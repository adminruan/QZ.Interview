using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using QZ.Common;
using QZ.Model.Expand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QZ.Interview.Api.Bases
{
    /// <summary>
    /// 相关过滤
    /// </summary>
    public class InterviewFilterAttribute : Attribute, IAuthorizationFilter, IActionFilter
    {
        /// <summary>
        /// 签名校验 优先级1
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor && actionDescriptor.MethodInfo.GetCustomAttributes(inherit: false).Any(p => p.GetType().Equals(typeof(NotSignVerifyAttribute))))
            {
                //无需校验签名
                return;
            }
            if (ValidSign(context.HttpContext))
            {
                //通过签名校验
                return;
            }
            RedirectResult redirectResult = new RedirectResult("~/Error/SignError/?msg=" + QZ_Helper_URLUtils.UrlEncode("检测到网络不安全或签名错误,拒绝访问!") + "&IP=" + context.HttpContext.GetClientUserIp());
            context.Result = redirectResult;
            return;
        }

        /// <summary>
        /// 模型参数校验 优先级3
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                Response_BaseModel data = new Response_BaseModel();
                data.SIP = QZ_Helper_Encryption.Base64Encode(QZ_Helper_IP.GetServiceIP());
                data.Date = QZ_Helper_Encryption.Base64Encode(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                data.S = QZ_Helper_Encryption.Base64Encode("0");
                foreach (var item in context.ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        data.msg += error.ErrorMessage + "|";
                    }
                }
                data.msg = QZ_Helper_Encryption.Base64Encode(data.msg.Substring(0, data.msg.Length - 1));
                context.Result = new JsonResult(data);
            }
            return;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        /// <summary>
        /// 验证签名方法
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public bool ValidSign(HttpContext context)
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            Dictionary<string, string> keyUrlDecode = new Dictionary<string, string>();
            var queryRequest = context.Request;
            string path = queryRequest.Path;
            if (queryRequest.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) && queryRequest.ContentLength != 0)
            {
                foreach (string s in queryRequest.Form.Keys.ToList())
                {
                    keyValues.Add(s, (queryRequest.Form[s].ToString()));
                    keyUrlDecode.Add(s, HttpUtility.UrlDecode(queryRequest.Form[s].ToString()));
                }
            }
            else
            {
                foreach (string s in queryRequest.Query.Keys)
                {
                    keyValues.Add(s, (queryRequest.Query[s].ToString()));
                    keyUrlDecode.Add(s, HttpUtility.UrlDecode(queryRequest.Query[s].ToString()));
                }
            }

            string sign = keyValues.FirstOrDefault(s => s.Key.Equals("sign")).Value;
            keyValues.Remove("sign");
            keyUrlDecode.Remove("sign");
            string signKey = QZ_Helper_Validate.GetSignature(keyValues, null, out string baseString);
            string signKeyUndefined = QZ_Helper_Validate.GetSignatureNoUndefined(keyUrlDecode, null, out string baseStringUndefined);
            string signKeyReplace = QZ_Helper_Validate.GetSignatureNoReplace(keyUrlDecode, null, out string baseStringReplace);
            if (sign != signKey && sign != signKeyUndefined && sign != signKeyReplace)
            {
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 无需校验签名特性
    /// </summary>
    public class NotSignVerifyAttribute : Attribute
    {

    }
}
