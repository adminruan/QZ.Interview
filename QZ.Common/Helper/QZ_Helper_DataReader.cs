using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace QZ.Common
{
    public static class QZ_Helper_DataReader
    {

        public static IList<T> ConvertTo<T>(DataTable table)
        {
            if (table == null)
            {
                return null;
            }

            List<DataRow> rows = new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            return ConvertTo<T>(rows);
        }

        public static IList<T> ConvertTo<T>(IList<DataRow> rows)
        {
            IList<T> list = null;

            if (rows != null)
            {
                list = new List<T>();

                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }

            return list;
        }

        public static T CreateItem<T>(DataRow row)
        {
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                    try
                    {
                        if (prop != null)
                        {
                            object value = row[column.ColumnName];
                            prop.SetValue(obj, value, null);
                        }
                    }
                    catch
                    {  //You can log something here     
                       //throw;    
                    }
                }
            }

            return obj;
        }


        public static T ReaderToEntity<T>(IDataReader dr)
        {
            using (dr)
            {
                if (dr.Read())
                {
                    Type modelType = typeof(T);
                    int count = dr.FieldCount;
                    T model = Activator.CreateInstance<T>();
                    for (int i = 0; i < count; i++)
                    {
                        if (!IsNullOrDBNull(dr[i]))
                        {
                            PropertyInfo pi = modelType.GetProperty(dr.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                            if (pi != null)
                            {
                                pi.SetValue(model, HackType(dr[i], pi.PropertyType), null);
                            }
                        }
                    }
                    return model;
                }
            }
            return default(T);
        }

        public static List<T> ReaderToList<T>(IDataReader dr)
        {
            using (dr)
            {
                List<T> list = new List<T>();
                Type modelType = typeof(T);
                int count = dr.FieldCount;
                while (dr.Read())
                {
                    T model = Activator.CreateInstance<T>();

                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            if (!IsNullOrDBNull(dr[i]))
                            {
                                //GetPropertyName(dr.GetName(i))
                                PropertyInfo pi = modelType.GetProperty(dr.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                if (pi != null)
                                {
                                    pi.SetValue(model, HackType(dr[i], pi.PropertyType), null);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    list.Add(model);
                }
                return list;
            }
        }

        public static List<T> ReaderToList<T>(DbDataReader dr)
        {
            using (dr)
            {
                List<T> list = new List<T>();
                Type modelType = typeof(T);
                int count = dr.FieldCount;
                while (dr.Read())
                {
                    T model = Activator.CreateInstance<T>();
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            if (!IsNullOrDBNull(dr[i]))
                            {
                                PropertyInfo pi = modelType.GetProperty(dr.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                if (pi != null)
                                {
                                    pi.SetValue(model, HackType(dr[i], pi.PropertyType), null);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    list.Add(model);
                }
                return list;
            }
        }

        public static DataTable ConvertDataReaderToDataTable(DbDataReader dataReader)
        {
            ///定义DataTable
            DataTable datatable = new DataTable();

            try
            {    ///动态添加表的数据列
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    DataColumn myDataColumn = new DataColumn();
                    myDataColumn.DataType = dataReader.GetFieldType(i);
                    myDataColumn.ColumnName = dataReader.GetName(i);
                    datatable.Columns.Add(myDataColumn);
                }

                ///添加表的数据
                while (dataReader.Read())
                {
                    DataRow myDataRow = datatable.NewRow();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        myDataRow[i] = dataReader[i].ToString();
                    }
                    datatable.Rows.Add(myDataRow);
                    myDataRow = null;
                }
                ///关闭数据读取器
                dataReader.Close();
                return datatable;
            }
            catch (Exception ex)
            {
                ///抛出类型转换错误
                //SystemError.CreateErrorLog(ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }

        //这个类对可空类型进行判断转换，要不然会报错   
        private static object HackType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;

                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            return Convert.ChangeType(value, conversionType);

        }
        private static bool IsNullOrDBNull(object obj)
        {
            return (obj == null || (obj is DBNull)) ? true : false;
        }

        //取得DB的列对应bean的属性名   

        private static string GetPropertyName(string column)
        {

            column = column.ToLower();

            string[] narr = column.Split('_');

            column = "";

            for (int i = 0; i < narr.Length; i++)
            {

                if (narr[i].Length > 1)
                {
                    column += narr[i].Substring(0, 1).ToUpper() + narr[i].Substring(1);

                }
                else
                {
                    column += narr[i].Substring(0, 1).ToUpper();

                }
            }
            return column;
        }

        /// <summary>
        /// 将 DataReader 填充到 DataTable 的函数
        /// </summary>
        /// <param name="oDataReader">SqlDateReader 数据集合</param>
        /// <param name="inTableNum"> 要返回的结果集的编号，从0开始，默认为第一个结果集 </param>
        /// <returns>DateTable 数据集合</returns>
        /// <remarks>带一个必选参数，转换函数</remarks>
        public static System.Data.DataTable CDataTable(ref IDataReader oDataReader, int inTableNum)
        {

            //验证需要返回的结果集的序号，指定返回第几个数据集（第一个数据集为0）
            if (inTableNum != 0)
            {
                int i = 0;
                for (i = 1; i <= inTableNum; i++)
                {
                    oDataReader.NextResult();
                }
            }

            int iLoop = 0;
            System.Data.DataTable oDataTable = new System.Data.DataTable();
            //临时缓存表数据
            System.Data.DataTable oSchemaTable = new System.Data.DataTable();
            //临时缓存表结构
            System.Data.DataRow oDataRow = null;

            //复制表结构
            oSchemaTable = oDataReader.GetSchemaTable();

            //将表结构读取到临时数据表中
            for (iLoop = 0; iLoop <= oSchemaTable.Rows.Count - 1; iLoop++)
            {
                oDataTable.Columns.Add(Convert.ToString(oSchemaTable.Rows[iLoop]["ColumnName"]), (System.Type)oSchemaTable.Rows[iLoop]["DataType"]);
            }

            while (oDataReader.Read())
            {
                oDataRow = oDataTable.NewRow();

                for (iLoop = 0; iLoop <= oSchemaTable.Rows.Count - 1; iLoop++)
                {
                    oDataRow[iLoop] = oDataReader[Convert.ToString(oSchemaTable.Rows[iLoop]["ColumnName"])];
                }

                oDataTable.Rows.Add(oDataRow);
            }

            oDataReader.Close();
            oSchemaTable.Rows.Clear();
            //oSchemaTable.Dispose();

            return oDataTable;
        }

    }
}
