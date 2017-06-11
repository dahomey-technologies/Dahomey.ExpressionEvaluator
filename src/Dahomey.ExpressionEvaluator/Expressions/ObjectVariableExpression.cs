#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

using System;
using System.Collections.Generic;

namespace Dahomey.ExpressionEvaluator
{
    public class ObjectVariableExpression : IObjectExpression
    {
        private string variableName;
        public Type ObjectType { get; private set; }

        public ObjectVariableExpression(string variableName, Type variableType)
        {
            this.variableName = variableName;
            ObjectType = variableType;
        }

        public object GetInstance(Dictionary<string, object> variables)
        {
            return variables[variableName];
        }

        public override string ToString()
        {
            return variableName;
        }
    }
}
