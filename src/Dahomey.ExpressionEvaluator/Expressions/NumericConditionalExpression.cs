#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

 using System.Collections.Generic;
using System.Text;

namespace Dahomey.ExpressionEvaluator
{
    public class NumericConditionalExpression : INumericExpression
    {
        public IBooleanExpression ConditionExpr { get; set; }
        public INumericExpression LeftExpr { get; set; }
        public INumericExpression RightExpr { get; set; }

        public double Evaluate(Dictionary<string, object> variables)
        {
            return ConditionExpr.Evaluate(variables) ? LeftExpr.Evaluate(variables) : RightExpr.Evaluate(variables);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (ConditionExpr != null)
            {
                sb.Append(LeftExpr).Append(' ');
            }

            if (LeftExpr != null)
            {
                sb.Append(LeftExpr).Append(' ');
            }

            if (RightExpr != null)
            {
                sb.Append(RightExpr).Append(' ');
            }

            sb.Append("?:");

            return sb.ToString();
        }
    }
}
