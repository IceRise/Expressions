using System;
using System.Linq.Expressions;

namespace Expressions
{
    public interface IExpressionCondition<TEntity>
    {
        Expression<Func<TEntity, object>> Field { get; set; }

        Operator Operator { get; set; }

        object Value { get; set; }

        // This is used with strings only
        bool CaseSensitive { get; set; }
    }
}