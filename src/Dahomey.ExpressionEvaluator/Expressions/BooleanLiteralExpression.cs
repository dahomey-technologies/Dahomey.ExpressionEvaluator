#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

 using System.Collections.Generic;

namespace Dahomey.ExpressionEvaluator
{
    public class BooleanLiteralExpression : IBooleanExpression
    {
        private bool value;

        public BooleanLiteralExpression(bool value)
        {
            this.value = value;
        }

        public bool Evaluate(Dictionary<string, object> variables)
        {
            return value;
        }

        public override string ToString()
        {
            return value ? "true" : "false";
        }
    }
}
