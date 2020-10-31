using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace QZ.Common
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public static class QZ_Helper_EnumHelper
    {
        /// <summary>
        /// 获取枚举对应中文描述
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum obj)
        {
            try
            {
                string enumName = obj.ToString();
                Type t = obj.GetType();
                FieldInfo field = t.GetField(enumName);//获取枚举字段
                DescriptionAttribute[] desArry = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);//获取字段所有描述
                return desArry[0].Description;//返回第一个描述
            }
            catch (Exception)
            {
                return "未知类型";
            }
        }

        /// <summary>
        /// 通过描述获取对应枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="des"></param>
        /// <param name="t"></param>
        public static void GetEnum<T>(string des, ref T t) where T : Enum
        {
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                string thisDes = GetEnumDescription(item as Enum);
                if (thisDes == des || thisDes.Contains(des))
                {
                    t = item;
                }
            }
        }

        /// <summary>
        /// 将枚举转化成字典格式
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static Dictionary<string, int> ToPairs(Type type)
        {
            Dictionary<string, int> pairs = new Dictionary<string, int>();
            FieldInfo[] fieldInfos = type.GetFields();//得到枚举所有字段
            string key = string.Empty;
            int value = 0;
            foreach (FieldInfo field in fieldInfos)
            {
                if (field.FieldType.IsEnum)
                {
                    try
                    {
                        key = ((DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false))[0].Description;
                    }
                    catch (Exception)
                    {
                        key = field.Name;
                    }
                    value = Convert.ToInt32(type.InvokeMember(field.Name, BindingFlags.GetField, null, null, null));
                    pairs.Add(key, value);
                }
            }
            return pairs;
        }
    }
}
