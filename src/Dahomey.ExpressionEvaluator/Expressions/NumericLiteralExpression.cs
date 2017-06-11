#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

 using System.Collections.Generic;
using System.Globalization;

namespace Dahomey.ExpressionEvaluator
{
    public class NumericLiteralExpression : INumericExpression
    {
        private double value;

        public NumericLiteralExpression(double value)
        {
            this.value = value;
        }

        public double Evaluate(Dictionary<string, object> variables)
        {
            return value;
        }

        public override string ToString()
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
