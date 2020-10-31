using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Model.Expand.Wechat
{
    public class WeChat_Model_AccessToken
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }
    }
}
