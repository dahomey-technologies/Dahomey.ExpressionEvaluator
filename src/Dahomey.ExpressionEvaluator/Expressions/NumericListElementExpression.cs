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
    public class NumericListElementExpression : INumericExpression
    {
        private readonly Func<object, int, double> evaluator;
        private IObjectExpression listExpr;
        public INumericExpression indexExpr;

        public NumericListElementExpression(IObjectExpression listExpr, INumericExpression indexExpr)
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

            Func<Func<object, int, double>> generateEvaluatorDelegate =
                ReflectionHelper.CreateDelegate<Func<object, int, double>>(generateEvaluatorMethod);

            evaluator = generateEvaluatorDelegate();
        }

        public double Evaluate(Dictionary<string, object> variables)
        {
            object listInstance = listExpr.GetInstance(variables);
            return evaluator(listInstance, (int)indexExpr.Evaluate(variables));
        }

        private static Func<object, int, double> GenerateEvaluator<TI>()
        {
            Func<TI, double> converter = ReflectionHelper.GenerateConverter<TI>();
            return (obj, idx) => converter(((IList<TI>)obj)[idx]);
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", listExpr, indexExpr);
        }
    }
}
