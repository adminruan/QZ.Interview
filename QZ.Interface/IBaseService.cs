using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace QZ.Interface
{
    public interface IBaseService : IDisposable
    {
        /// <summary>
        /// 根据id查询实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Find<T>(int id, bool isRead = true) where T : class;

        /// <summary>
        /// 获取默认一条数据，没有则为NULL
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        T FirstOrDefault<T>(Expression<Func<T, bool>> whereLambda = null, bool isRead = true) where T : class;


        /// <summary>
        /// 带条件查询获取数据
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        IQueryable<T> GetAllIQueryable<T>(Expression<Func<T, bool>> whereLambda = null, bool isRead = true) where T : class;

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="whereLambd"></param>
        /// <returns></returns>
        int GetCount<T>(Expression<Func<T, bool>> whereLambd = null, bool isRead = true) where T : class;

        /// <summary>
        /// 判断对象是否存在
        /// </summary>
        /// <param name="whereLambd"></param>
        /// <returns></returns>
        bool Any<T>(Expression<Func<T, bool>> whereLambd) where T : class;

        /// <summary>
        /// 提供对单表的查询，
        /// </summary>
        /// <returns>IQueryable类型集合</returns>
        IQueryable<T> Set<T>() where T : class;

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcWhere"></param>
        /// <returns></returns>
        IQueryable<T> Query<T>(Expression<Func<T, bool>> funcWhere, bool isRead = true) where T : class;

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="funcWhere"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="funcOrderby"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        PageResult<T> QueryPage<T, S>(Expression<Func<T, bool>> funcWhere, int pageSize, int pageIndex, Expression<Func<T, S>> funcOrderby, bool isAsc = true) where T : class;

        /// <summary>
        /// 这是一个假分页，适用于少数据量的表。SQL语句是查询了全部在将数据进行分页的 (大数据量的用下面存储过程分页方法(GetPageInstanceBySql))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="isRead"></param>
        /// <returns></returns>
        PageResult<T> ExcuteQueryPage<T>(string sql, int pageSize, int pageIndex, bool isRead = true) where T : class, new();

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

        PageResult<T> GetPageInstanceBySql<T>(string sql, int pageSize, int pageIndex, string configName) where T : class, new();

        /// <summary>
        /// 新增数据，即时Commit
        /// </summary>
        /// <param name="t"></param>
        /// <returns>返回带主键的实体</returns>
        T Insert<T>(T t) where T : class;

        /// <summary>
        /// 新增数据，即时Commit
        /// 多条sql 一个连接，事务插入
        /// </summary>
        /// <param name="tList"></param>
        /// <returns>返回带主键的集合</returns>
        IEnumerable<T> Insert<T>(IEnumerable<T> tList) where T : class;

        /// <summary>
        /// 更新数据，即时Commit
        /// </summary>
        /// <param name="t"></param>
        void Update<T>(T t) where T : class;

        /// <summary>
        /// 更新数据，即时Commit
        /// </summary>
        /// <param name="tList"></param>
        void Update<T>(IEnumerable<T> tList) where T : class;

        /// <summary>
        /// 根据主键删除数据，即时Commit
        /// </summary>
        /// <param name="t"></param>
        void Delete<T>(int Id) where T : class;

        /// <summary>
        /// 删除数据，即时Commit
        /// </summary>
        /// <param name="t"></param>
        void Delete<T>(T t) where T : class;

        /// <summary>
        /// 删除数据，即时Commit
        /// </summary>
        /// <param name="tList"></param>
        void Delete<T>(IEnumerable<T> tList) where T : class;

        /// <summary>
        /// 立即保存全部修改
        /// </summary>
        void Commit();

        /// <summary>
        /// 执行sql 返回集合
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        List<T> ExcuteQuery<T>(string sql, SqlParameter[] parameters, bool isRead = true) where T : class, new();

        List<T> ExcuteQuery<T>(string sql, bool isRead = true) where T : class, new();


        /// <summary>
        /// 执行sql，无返回
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        void Excute<T>(string sql, SqlParameter[] parameters) where T : class;


        /// <summary>
        /// 执行sql，无返回（注意：这个方法只走写的数据库）
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        void Excute<T>(string sql) where T : class;

        List<T> ExcuteStoredProcedure<T>(string sql, bool isRead = true) where T : class, new();

        DataTable SqlQueryForDataTatable(string sql, bool isRead = true);
    }

    public class PageResult<T>
    {
        /// <summary>
        /// 总数据量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页数据量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页数据集合
        /// </summary>
        public List<T> DataList { get; set; }
    }
}
