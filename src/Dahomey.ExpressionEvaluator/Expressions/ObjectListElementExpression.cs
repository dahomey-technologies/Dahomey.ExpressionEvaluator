#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dahomey.ExpressionEvaluator
{
    public class ObjectListElementExpression : IObjectExpression
    {
        private readonly Func<object, int, object> evaluator;
        private IObjectExpression listExpr;
        public INumericExpression indexExpr;
        public Type ObjectType { get; private set; }

        public ObjectListElementExpression(IObjectExpression listExpr, INumericExpression indexExpr)
        {
            this.listExpr = listExpr;
            this.indexExpr = indexExpr;

            Type objectType = listExpr.ObjectType;
            Type itemType;

            if (objectType.IsArray)
            {
                itemType = objectType.GetElementType();
            }
            else if (ReflectionHelper.IsList(objectType))
            {
                itemType = objectType.GetGenericArguments()[0];
            }
            else
            {
                throw new NotSupportedException();
            }

            MethodInfo generateEvaluatorMethod = GetType()
                .GetMethod("GenerateEvaluator", BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(itemType);

            Func<Func<object, int, object>> generateEvaluatorDelegate =
                ReflectionHelper.CreateDelegate<Func<object, int, object>>(generateEvaluatorMethod);

            evaluator = generateEvaluatorDelegate();
        }

        public object GetInstance(Dictionary<string, object> variables)
        {
            object listInstance = listExpr.GetInstance(variables);
            return evaluator(listInstance, (int)indexExpr.Evaluate(variables));
        }

        private static Func<object, int, object> GenerateEvaluator<TI>()
        {
            return (obj, idx) => ((IList<TI>)obj)[idx];
        }
    }
}
