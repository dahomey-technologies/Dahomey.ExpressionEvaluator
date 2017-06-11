#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

using System;
using System.Collections.Generic;

namespace Dahomey.ExpressionEvaluator
{
    public class EnumLiteralExpression : INumericExpression
    {
        private Enum enumValue;
        private double value;

        public EnumLiteralExpression(Enum value)
        {
            this.value = Convert.ToDouble(value);
            this.enumValue = value;
        }

        public double Evaluate(Dictionary<string, object> variables)
        {
            return value;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", enumValue.GetType().Name, enumValue.ToString());
        }
    }
}
