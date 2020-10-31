using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace QZ.Common.Expand
{
    public static class QZ_Expand_Expression
    {
        private static Expression<T> Combine<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            NewExpressionVisitor visitor = new NewExpressionVisitor(first.Parameters[0]);
            Expression body1 = visitor.Visit(first.Body);
            Expression body2 = visitor.Visit(second.Body);
            return Expression.Lambda<T>(merge(body1, body2), first.Parameters[0]);
        }

        /// <summary>
        /// 表达式合并-并且
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Combine(second, Expression.And);
        }

        /// <summary>
        /// 表达式合并-或者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Combine(second, Expression.Or);
        }
    }

    public class NewExpressionVisitor : ExpressionVisitor
    {
        public NewExpressionVisitor(ParameterExpression parameter)
        {
            this._Parameter = parameter;
        }
        public ParameterExpression _Parameter { get; set; }

        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            return _Parameter;
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }
    }
}
