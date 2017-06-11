#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

using System;
using System.Collections.Generic;

namespace Dahomey.ExpressionEvaluator
{
    public class NumericVariableExpression : INumericExpression
    {
        private string variableName;
        private Type variableType;

        public NumericVariableExpression(string variableName, Type variableType)
        {
            this.variableName = variableName;
            this.variableType = variableType;
        }

        public double Evaluate(Dictionary<string, object> variables)
        {
            return Convert.ToDouble(variables[variableName]);
        }

        public override string ToString()
        {
            return variableName;
        }
    }
}
