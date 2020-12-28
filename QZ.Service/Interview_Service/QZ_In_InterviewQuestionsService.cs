using Microsoft.EntityFrameworkCore;
using QZ.Common.Enums;
using QZ.Common.Expand;
using QZ.Interface.Interview_IService;
using QZ.Model.DBContext;
using QZ.Model.Interview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QZ.Service.Interview_Service
{
    public class QZ_In_InterviewQuestionsService : BaseService, QZ_In_IInterviewQuestionsService
    {
        private readonly DbSet<QZ_Model_In_InterviewQuestions> _InterviewQuestions;
        public QZ_In_InterviewQuestionsService(Interview_DB_EFContext dbContext) : base(dbContext)
        {
            this._InterviewQuestions = dbContext.InterviewQuestions;
        }
    }
}
