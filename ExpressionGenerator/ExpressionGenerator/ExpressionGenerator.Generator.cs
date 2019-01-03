using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Expressions
{
    public partial class ExpressionGenerator<TEntity>
    {
        public Func<TEntity, bool> GenerateExpression(IExpressionCondition<TEntity>[] filters)
        {
            Expression<Func<TEntity, bool>> resultExpression = null;
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity));
            resultExpression = GetResultExpression(filters, parameter, resultExpression);

            return resultExpression.Compile();
        }

        public Func<TEntity, bool> GenerateExpression<T>(
            Expression<Func<TEntity, T>> fieldExpression,
            T value,
            Operator filterOperator,
            bool caseSensitive = true)
        {
            BuiltExpression expression = BuildExpression(fieldExpression, value, filterOperator, caseSensitive);
            var result = Expression.Lambda<Func<TEntity, bool>>(expression.Condition, expression.Parameter);
            return result.Compile();
        }

        private Expression<Func<TEntity, bool>> GetResultExpression(IExpressionCondition<TEntity>[] filterConditions, ParameterExpression parameter, Expression<Func<TEntity, bool>> resultExpression)
        {
            var visitor = new ParameterVisitor(parameter);
            foreach (IExpressionCondition<TEntity> condition in filterConditions)
            {
                BuiltExpression expression = BuildExpression(condition.Field, condition.Value, condition.Operator, condition.CaseSensitive);
                visitor.AddParamToReplace(expression.Parameter);
                if (resultExpression == null)
                {
                    resultExpression = Expression.Lambda<Func<TEntity, bool>>(visitor.Visit(expression.Condition), parameter);
                    continue;
                }

                var exp = Expression.And(resultExpression.Body, visitor.Visit(expression.Condition));
                resultExpression = Expression.Lambda<Func<TEntity, bool>>(exp, parameter);
            }

            return resultExpression;
        }

        private static Expression GetStringRelatedExpression(
            Expression member,
            Expression propertyValue,
            string methodName,
            Type[] parameterTypes)
        {
            MethodInfo method = typeof(string).GetMethod(methodName, parameterTypes);
            return Expression.Call(member, method, propertyValue);
        }

        private static Expression GetIgnoreCaseStringExpression(Expression member, Expression propertyValue, string methodName)
        {
            if (methodName == "Contains")
            {
                var indexOfExpression = GetStringRelatedExpression(member,
                    propertyValue,
                    "IndexOf",
                    new[] { typeof(string), typeof(StringComparison) });
                return Expression.GreaterThanOrEqual(indexOfExpression, Expression.Constant(0));
            }

            return GetStringRelatedExpression(member, propertyValue, methodName, new[] { typeof(string), typeof(StringComparison) });
        }

        private BuiltExpression BuildExpression<T>(
            Expression<Func<TEntity, T>> fieldExpression,
            T value,
            Operator filterOperator,
            bool filterCaseSensitive)
        {
            MemberExpression memberExpression = fieldExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                if (fieldExpression.Body is UnaryExpression unary)
                {
                    memberExpression = unary.Operand as MemberExpression;
                }
            }

            if (memberExpression == null)
            {
                ParameterExpression fieldParameter = fieldExpression.Parameters[0];
                var parameter = Expression.Constant(value);
                Expression cond = GetCondition(
                    filterOperator,
                    fieldExpression.Body,
                    parameter,
                    filterCaseSensitive,
                    fieldExpression.Body.Type == typeof(string));
                return new BuiltExpression(fieldParameter, cond);
            }

            if (memberExpression.Type.IsEnum)
            {
                value = (T)Enum.ToObject(memberExpression.Type, value);
            }

            ParameterExpression objParam = fieldExpression.Parameters[0];
            var param = Expression.Constant(value);
            var convertedParameter = Expression.Convert(param, memberExpression.Type);
            Expression condition = GetCondition(
                filterOperator,
                memberExpression,
                convertedParameter,
                filterCaseSensitive,
                memberExpression.Type == typeof(string));
            return new BuiltExpression(objParam, condition);
        }

        private Expression GetCondition(
            Operator operation,
            Expression member,
            Expression parameter,
            bool filterCaseSensitive,
            bool isString)
        {
            if (filterCaseSensitive && isString && _caseSensitiveOperators.Contains(operation))
            {
                return _caseSensitiveOperations[operation](member, parameter);
            }

            return _operations[operation](member, parameter);
        }

        private class BuiltExpression
        {
            public Expression Condition { get; }

            public ParameterExpression Parameter { get; }

            public BuiltExpression(ParameterExpression parameter, Expression condition)
            {
                Parameter = parameter;
                Condition = condition;
            }
        }
    }
}