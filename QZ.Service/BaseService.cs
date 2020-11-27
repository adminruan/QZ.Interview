using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QZ.Common;
using QZ.Common.Expand;
using QZ.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace QZ.Service
{
    public abstract class BaseService : IBaseService
    {
        #region Identity
        private static readonly object o = new object();
        protected DbContext _DbContext { get; private set; }

        public BaseService(DbContext context)
        {
            this._DbContext = context;
        }
        #endregion Identity


        public T Insert<T>(T t) where T : class
        {
            this._DbContext.Set<T>().Add(t);
            this.Commit();
            return t;
        }

        public IEnumerable<T> Insert<T>(IEnumerable<T> tList) where T : class
        {
            this._DbContext.Set<T>().AddRange(tList);
            this.Commit();//一个链接  多个sql
            return tList;
        }

        #region Query
        public T Find<T>(int id, bool isRead = true) where T : class
        {
            if (isRead)
            {
                return this._DbContext.Set<T>().Find(id);

            }
            else
            {
                return this._DbContext.Set<T>().Find(id);
            }
        }




        /// <summary>
        /// 获取默认一条数据，没有则为NULL
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public T FirstOrDefault<T>(Expression<Func<T, bool>> whereLambda = null, bool isRead = true) where T : class
        {
            if (isRead)
            {
                if (whereLambda == null)
                {
                    return this._DbContext.Set<T>().FirstOrDefault();
                }
                return this._DbContext.Set<T>().FirstOrDefault(whereLambda);
            }
            else
            {
                if (whereLambda == null)
                {
                    return this._DbContext.Set<T>().FirstOrDefault();
                }
                return this._DbContext.Set<T>().FirstOrDefault(whereLambda);
            }

        }


        /// <summary>
        /// 带条件查询获取数据
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public IQueryable<T> GetAllIQueryable<T>(Expression<Func<T, bool>> whereLambda = null, bool isRead = true) where T : class
        {
            if (isRead)
            {
                return whereLambda == null ? this._DbContext.Set<T>() : this._DbContext.Set<T>().Where(whereLambda);
            }
            else
            {
                return whereLambda == null ? this._DbContext.Set<T>() : this._DbContext.Set<T>().Where(whereLambda);
            }
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="whereLambd"></param>
        /// <returns></returns>
        public int GetCount<T>(Expression<Func<T, bool>> whereLambd = null, bool isRead = true) where T : class
        {
            if (isRead)
            {
                return whereLambd == null ? this._DbContext.Set<T>().Count() : this._DbContext.Set<T>().Where(whereLambd).Count();
            }
            else
            {
                return whereLambd == null ? this._DbContext.Set<T>().Count() : this._DbContext.Set<T>().Where(whereLambd).Count();
            }
        }


        /// <summary>
        /// 判断对象是否存在
        /// </summary>
        /// <param name="whereLambd"></param>
        /// <returns></returns>
        public bool Any<T>(Expression<Func<T, bool>> whereLambd) where T : class
        {
            return this._DbContext.Set<T>().Where(whereLambd).Any();
        }

        /// <summary>
        /// 不应该暴露给上端使用者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQueryable<T> Set<T>() where T : class
        {
            return this._DbContext.Set<T>();
        }

        /// <summary>
        /// 这才是合理的做法，上端给条件，这里查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        /// <returns></returns>
        public IQueryable<T> Query<T>(Expression<Func<T, bool>> funcWhere, bool isRead = true) where T : class
        {
            if (isRead)
            {
                return this._DbContext.Set<T>().Where<T>(funcWhere);
            }
            else
            {
                return this._DbContext.Set<T>().Where<T>(funcWhere);

            }
        }


        public PageResult<T> QueryPage<T, S>(Expression<Func<T, bool>> funcWhere, int pageSize, int pageIndex, Expression<Func<T, S>> funcOrderby, bool isAsc = true) where T : class
        {
            var list = this.Set<T>();
            if (funcWhere != null)
            {
                list = list.Where<T>(funcWhere).AsQueryable();
            }

            if (isAsc)
            {
                list = list.OrderBy(funcOrderby);
            }
            else
            {
                list = list.OrderByDescending(funcOrderby);
            }

            if (list.Count() > 0)
            {
                PageResult<T> result = new PageResult<T>();

                result.DataList = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                result.PageIndex = pageIndex;
                result.PageSize = pageSize;
                result.TotalCount = list.Count();
                result.PageCount = (list.Count() + pageSize - 1) / pageSize;

                return result;
            }
            return null;
        }

        /// <summary>
        /// 这是一个假分页，适用于少数据量的表。SQL语句是查询了全部在将数据进行分页的 (大数据量的用下面存储过程分页方法(GetPageInstanceBySql))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="isRead"></param>
        /// <returns></returns>
        public PageResult<T> ExcuteQueryPage<T>(string sql, int pageSize, int pageIndex, bool isRead = true) where T : class, new()
        {
            List<T> list = null;
            if (isRead)
            {

                list = this._DbContext.Database.SqlQuery<T>(sql);

            }
            else
            {
                list = this._DbContext.Database.SqlQuery<T>(sql);
            }

            if (list.Count() > 0)
            {
                PageResult<T> result = new PageResult<T>()
                {
                    TotalCount = list.Count(),
                    PageCount = (list.Count() + pageSize - 1) / pageSize,
                    DataList = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                return result;
            }
            return null;
        }

        /// <summary>
        /// 
        /// <param name="inSPName"> 存储过程名称 </param>
        /// <param name="inOutParameters"> 输入和输出的参数和返回值的集合 </param>
        /// <summary>
        /// 【方法】【函数】 SP_ReturnTable(inSPName, inOutParameters) - DataReader 执行 存储过程 返回 PageResult<T></summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">需要分页的SQL</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="configName">数据库连接字符串</param>
        /// <returns></returns>

        public PageResult<T> GetPageInstanceBySql<T>(string sql, int pageSize, int pageIndex, string configName) where T : class, new()
        {
            SqlParameter[] paras = new SqlParameter[] {
                            new SqlParameter("@sql",sql),
                            new SqlParameter("@currentPage",pageIndex ),
                            new SqlParameter("@pageSize",pageSize),
                             new SqlParameter("@recordcount",System.Data.SqlDbType.Int),
                              new SqlParameter("@pagecount",System.Data.SqlDbType.Int)
            };

            using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(configName))
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("USP_SplitPage", con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(paras);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        DataTable dt = ds.Tables[1];
                        int pageCount = (int)ds.Tables[2].Rows[0][0];
                        int countNum = (int)ds.Tables[3].Rows[0][0];
                        if (countNum > 0 && dt.Rows.Count > 0)
                        {
                            PageResult<T> result = new PageResult<T>()
                            {
                                TotalCount = countNum,
                                PageCount = pageCount,
                                DataList = QZ_Helper_DataReader.ConvertTo<T>(dt).ToList(),
                                PageIndex = pageIndex,
                                PageSize = pageSize
                            };
                            return result;
                        }
                        return null;

                    }
                }
            }

        }

        #endregion

        /// <summary>
        /// 是没有实现查询，直接更新的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void Update<T>(T t) where T : class
        {
            if (t == null) throw new Exception("t is null");
            this._DbContext.Set<T>().Attach(t).State = EntityState.Unchanged;//将数据附加到上下文，支持实体修改和新实体，重置为UnChanged
            this._DbContext.Entry<T>(t).State = EntityState.Modified;
            this.Commit();//保存 然后重置为UnChanged
        }


        public void Update<T>(IEnumerable<T> tList) where T : class
        {
            foreach (var t in tList)
            {
                this._DbContext.Set<T>().Attach(t);
                this._DbContext.Entry<T>(t).State = EntityState.Modified;
            }
            this.Commit();
        }

        /// <summary>
        /// 先附加 再删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void Delete<T>(T t) where T : class
        {
            if (t == null) throw new Exception("t is null");
            this._DbContext.Set<T>().Attach(t);
            this._DbContext.Set<T>().Remove(t);
            this.Commit();
        }

        public void Delete<T>(int Id) where T : class
        {
            T t = this.Find<T>(Id, true);//也可以附加
            if (t == null) throw new Exception("t is null");
            this._DbContext.Set<T>().Remove(t);
            this.Commit();
        }

        public void Delete<T>(IEnumerable<T> tList) where T : class
        {
            foreach (var t in tList)
            {
                this._DbContext.Set<T>().Attach(t);
            }
            this._DbContext.Set<T>().RemoveRange(tList);
            this.Commit();
        }


        public void Commit()
        {
            //this.Context.Database.
            this._DbContext.SaveChanges();
        }

        /// <summary>
        ///  执行Sql语句查询返回List集合（适合查询全部字段）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<T> ExcuteQuery<T>(string sql, SqlParameter[] parameters, bool isRead = true) where T : class, new()
        {
            if (isRead)
            {
                return this._DbContext.Database.SqlQuery<T>(sql, parameters);
            }
            else
            {
                return this._DbContext.Database.SqlQuery<T>(sql, parameters);
            }
        }




        /// <summary>
        /// 执行Sql语句查询返回List集合（适合查询指定字段）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<T> ExcuteQuery<T>(string sql, bool isRead = true) where T : class, new()
        {
            if (isRead)
            {
                return this._DbContext.Database.SqlQuery<T>(sql);
            }
            else
            {
                return this._DbContext.Database.SqlQuery<T>(sql);
            }

        }

        /// <summary>
        /// （注意：这个方法只走写的数据库）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        public void Excute<T>(string sql, SqlParameter[] parameters) where T : class
        {
            lock (o)
            {
                IDbContextTransaction trans = null;
                try
                {
                    trans = this._DbContext.Database.BeginTransaction();
                    this._DbContext.Database.ExecuteSqlCommand(sql, parameters);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行sql，无返回（注意：这个方法只走写的数据库）
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        public void Excute<T>(string sql) where T : class
        {
            IDbContextTransaction trans = null;
            try
            {
                trans = this._DbContext.Database.BeginTransaction();
                this._DbContext.Database.ExecuteSqlCommand(sql);
                trans.Commit();
            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();
                throw ex;
            }
        }


        public List<T> ExcuteStoredProcedure<T>(string sql, bool isRead = true) where T : class, new()
        {
            if (isRead)
            {
                return this._DbContext.Database.SqlQuery<T>(sql);
            }
            else
            {
                return this._DbContext.Database.SqlQuery<T>(sql);
            }
        }

        /// <summary>
        /// 执行Sql返回DataTable（注意：这个方法只走读的数据库）
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="isRead"></param>
        /// <returns></returns>
        public DataTable SqlQueryForDataTatable(string sql, bool isRead = true)
        {
            System.Data.DataTable oDataTable = new DataTable();
            DbConnection conn = this._DbContext.Database.GetDbConnection();
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = sql;
            DbDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            oDataTable.Load(reader);
            reader.Close();
            conn.Close();
            return oDataTable;
        }

        /// <summary>
        /// 指定字段更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">实体</param>
        /// <param name="properties">需要更新的字段</param>
        /// <returns></returns>
        public bool PartUpdate<T>(T entity, string[] properties) where T : class
        {
            var thisEntity = _DbContext.Attach(entity);
            foreach (var p in properties)
            {
                thisEntity.Property(p).IsModified = true;
            }
            return _DbContext.SaveChanges() > 0;
        }


        public virtual void Dispose()
        {
            if (this._DbContext != null)
            {
                this._DbContext.Dispose();
            }
        }
    }
}
