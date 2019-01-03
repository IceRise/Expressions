using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Expressions
{
    public partial class ExpressionGenerator<TEntity>
    {
        private static readonly Dictionary<Operator, Func<Expression, Expression, Expression>> _operations =
            new Dictionary<Operator, Func<Expression, Expression, Expression>>
            {
                { Operator.Equal, Expression.Equal },
                { Operator.StartsWith, StartsWithExpression },
                { Operator.EndsWith, EndsWithExpression },
                { Operator.Contains, ContainsExpression },
                {
                    Operator.NotContains,
                    (source, propertyValue) => Expression.Not(ContainsExpression(source, propertyValue))
                },
                { Operator.GreaterThan, Expression.GreaterThan },
                { Operator.LessThan, Expression.LessThan },
                { Operator.NotEqual, Expression.NotEqual },
                { Operator.GreaterThanOrEqual, Expression.GreaterThanOrEqual },
                { Operator.LessThanOrEqual, Expression.LessThanOrEqual }
            };

        private static readonly Dictionary<Operator, Func<Expression, Expression, Expression>> _caseSensitiveOperations =
            new Dictionary<Operator, Func<Expression, Expression, Expression>>
            {
                { Operator.Equal, CaseSensitiveEqualsExpression },
                { Operator.NotEqual, (source, propertyValue) => Expression.Not(CaseSensitiveEqualsExpression(source, propertyValue)) },
                { Operator.StartsWith, CaseSensitiveStartsWithExpression },
                { Operator.EndsWith, CaseSensitiveEndsWithExpression },
                { Operator.Contains, CaseSensitiveContainsExpression },
                {
                    Operator.NotContains,
                    (source, propertyValue) => Expression.Not(CaseSensitiveContainsExpression(source, propertyValue))
                }
            };

        private static readonly Operator[] _caseSensitiveOperators =
        {
            Operator.Contains,
            Operator.NotContains,
            Operator.StartsWith,
            Operator.EndsWith,
            Operator.Equal,
            Operator.NotEqual
        };

        private static Func<Expression, Expression, Expression> _startsWithExpression;
        private static Func<Expression, Expression, Expression> StartsWithExpression
        {
            get
            {
                if (_startsWithExpression == null)
                {
                    _startsWithExpression = (s, s2) => GetStringRelatedExpression(s, s2, "StartsWith", new[] { typeof(string) });
                }

                return _startsWithExpression;
            }
        }

        private static Func<Expression, Expression, Expression> _endsWithExpression;
        private static Func<Expression, Expression, Expression> EndsWithExpression
        {
            get
            {
                if (_endsWithExpression == null)
                {
                    _endsWithExpression = (s, s2) => GetStringRelatedExpression(s, s2, "EndsWith", new[] { typeof(string) });
                }

                return _endsWithExpression;
            }
        }

        private static Func<Expression, Expression, Expression> _containsExpression;
        private static Func<Expression, Expression, Expression> ContainsExpression
        {
            get
            {
                if (_containsExpression == null)
                {
                    _containsExpression = (s, s2) => GetStringRelatedExpression(s, s2, "Contains", new[] { typeof(string) });
                }

                return _containsExpression;
            }
        }

        private static Func<Expression, Expression, Expression> _caseSensitiveContainsExpression;
        private static Func<Expression, Expression, Expression> CaseSensitiveContainsExpression
        {
            get
            {
                if (_caseSensitiveContainsExpression == null)
                {
                    _caseSensitiveContainsExpression = (s, s2) => GetIgnoreCaseStringExpression(s, s2, "Contains");
                }

                return _caseSensitiveContainsExpression;
            }
        }

        private static Func<Expression, Expression, Expression> _caseSensitiveStartsWithExpression;
        private static Func<Expression, Expression, Expression> CaseSensitiveStartsWithExpression
        {
            get
            {
                return _caseSensitiveStartsWithExpression ??
                      (_caseSensitiveStartsWithExpression = (s, s2) => GetIgnoreCaseStringExpression(s, s2, "StartsWith"));
            }
        }

        private static Func<Expression, Expression, Expression> _caseSensitiveEndsWithExpression;
        private static Func<Expression, Expression, Expression> CaseSensitiveEndsWithExpression
        {
            get
            {
                return _caseSensitiveEndsWithExpression ??
                      (_caseSensitiveEndsWithExpression = (s, s2) => GetIgnoreCaseStringExpression(s, s2, "EndsWith"));
            }
        }

        private static Func<Expression, Expression, Expression> _caseSensitiveEqualsExpression;
        private static Func<Expression, Expression, Expression> CaseSensitiveEqualsExpression
        {
            get
            {
                return _caseSensitiveEqualsExpression ??
                      (_caseSensitiveEqualsExpression = (s, s2) => GetIgnoreCaseStringExpression(s, s2, "Equals"));
            }
        }
    }
}