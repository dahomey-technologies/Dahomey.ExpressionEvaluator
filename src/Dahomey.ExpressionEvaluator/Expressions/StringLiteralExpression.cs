#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

 using System.Collections.Generic;
using System.Globalization;

namespace Dahomey.ExpressionEvaluator
{
    public class StringLiteralExpression : IStringExpression
    {
        private string value;

        public StringLiteralExpression(string value)
        {
            this.value = value;
        }

        public string Evaluate(Dictionary<string, object> variables)
        {
            return value;
        }

        public override string ToString()
        {
            return string.Format("\"{0}\"", value);
        }
    }
}
