using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Model.Expand.Wechat
{
    public class Watermark
    {
        public int timestamp { get; set; }

        public string appid { get; set; }
    }

    public class WeChat_Model_WXXCXUserInfo
    {
        /// <summary>
        /// openId
        /// </summary>
        public string openId { get; set; }

        /// <summary>
        /// 微信昵称
        /// </summary>
        public string nickName { get; set; }

        /// <summary>
        /// 微信头像
        /// </summary>
        public string avatarUrl { get; set; }

        /// <summary>
        /// 性别  0	未知  1	男性  2	女性
        /// </summary>
        public int gender { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        public string language { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string province { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        public string country { get; set; }

        /// <summary>
        /// 微信唯一ID
        /// </summary>
        public string unionId { get; set; }

        public Watermark watermark { get; set; }
    }
}
