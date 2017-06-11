#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Dahomey.ExpressionEvaluator
{
    public class StrngComparisonExpression : IBooleanExpression
    {
        public Operator Operator { get; set; }
        public IStringExpression LeftExpr { get; set; }
        public IStringExpression RightExpr { get; set; }

        public bool Evaluate(Dictionary<string, object> variables)
        {
            switch (Operator)
            {
                case Operator.Equal:
                    return LeftExpr.Evaluate(variables) == RightExpr.Evaluate(variables);

                case Operator.NotEqual:
                    return LeftExpr.Evaluate(variables) != RightExpr.Evaluate(variables);

                default:
                    throw new NotSupportedException(string.Format("Operator {0} not supported", Operator));
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (LeftExpr != null)
            {
                sb.Append(LeftExpr).Append(' ');
            }

            if (RightExpr != null)
            {
                sb.Append(RightExpr).Append(' ');
            }

            sb.Append(Operator.PrettyPrint());

            return sb.ToString();
        }
    }
}
