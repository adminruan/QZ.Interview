using Microsoft.EntityFrameworkCore;
using QZ.Interface.Interview_IService;
using QZ.Model.DBContext;
using QZ.Model.Interview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QZ.Service.Interview_Service
{
    public class QZ_In_PositionsService : BaseService, QZ_In_IPositionsService
    {
        public QZ_In_PositionsService(Interview_DB_EFContext dbContext) : base(dbContext)
        {
            this._Positions = dbContext.Positions;
        }
        private readonly DbSet<QZ_Model_In_Positions> _Positions;

        #region 读取
        /// <summary>
        /// 获取职位
        /// </summary>
        /// <returns></returns>
        public List<QZ_Model_In_Positions> GetPositions()
        {
            return _Positions.Where(p => p.State && p.PositionName != "总经理").ToList();
        }

        /// <summary>
        /// 通过ID获取职位信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public QZ_Model_In_Positions GetInfoByID(int id)
        {
            return _Positions.Find(id);
        }
        #endregion
    }
}
