using System.Collections.Generic;
using System.Linq.Expressions;

namespace Expressions
{
    internal class ParameterVisitor : ExpressionVisitor
    {
        public ParameterVisitor(ParameterExpression to)
        {
            _to = to;
        }

        private readonly ParameterExpression _to;
        private readonly List<ParameterExpression> _paramsToReplace = new List<ParameterExpression>();

        internal void AddParamToReplace(ParameterExpression from)
        {
            _paramsToReplace.Add(from);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _paramsToReplace.Contains(node) ? _to : base.VisitParameter(node);
        }
    }
}