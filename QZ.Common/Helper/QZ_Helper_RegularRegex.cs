using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace QZ.Common
{
    public class QZ_Helper_RegularRegex
    {
        /// <summary>
        /// 校验证件号
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public static bool CheckIdentity(string cid)
        {
            if (string.IsNullOrWhiteSpace(cid))
            {
                return false;
            }
            cid = string.IsNullOrWhiteSpace(cid) ? cid : cid.ToLower();
            string[] aCity = new string[] { null, null, null, null, null, null, null, null, null, null, null, "北京", "天津", "河北", "山西", "内蒙古", null, null, null, null, null, "辽宁", "吉林", "黑龙江", null, null, null, null, null, null, null, "上海", "江苏", "浙江", "安微", "福建", "江西", "山东", null, null, null, "河南", "湖北", "湖南", "广东", "广西", "海南", null, null, null, "重庆", "四川", "贵州", "云南", "西藏", null, null, null, null, null, null, "陕西", "甘肃", "青海", "宁夏", "新疆", null, null, null, null, null, "台湾", null, null, null, null, null, null, null, null, null, "香港", "澳门", null, null, null, null, null, null, null, null, "国外" };
            double iSum = 0;
            Regex rg = new Regex(@"^\d{17}(\d|x)$");
            Match mc = rg.Match(cid);
            if (!mc.Success)
            {
                return false;//必须为18位数字或x结尾
            }
            cid = cid.ToLower();
            cid = cid.Replace("x", "a");
            if (aCity[int.Parse(cid.Substring(0, 2))] == null)
            {
                return false;//非法地区
            }
            try
            {
                DateTime.Parse(cid.Substring(6, 4) + "-" + cid.Substring(10, 2) + "-" + cid.Substring(12, 2));
            }
            catch
            {
                return false;//非法生日
            }
            for (int i = 17; i >= 0; i--)
            {
                iSum += (Math.Pow(2, i) % 11) * int.Parse(cid[17 - i].ToString(), System.Globalization.NumberStyles.HexNumber);
            }
            if (iSum % 11 != 1)
                return false;//非法证号

            return true;
        }

        /// <summary>
        /// 校验手机号码（正确返回 true）
        /// </summary>
        /// <param name="phoneNum">手机号</param>
        /// <returns></returns>
        public static bool CheckPhoneNumber(string phoneNum)
        {
            if (string.IsNullOrWhiteSpace(phoneNum) || phoneNum.Length < 11)
            {
                return false;
            }
            Regex regex = new Regex(@"^1[3456789]\d{9}$");
            if (regex.IsMatch(phoneNum))
            {
                return true;
            }
            return false;
        }
    }
}
