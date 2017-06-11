using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dahomey.ExpressionEvaluator
{
    public class NumericPropertyExpression : INumericExpression
    {
        private IObjectExpression containingObject;
        private PropertyInfo propertyInfo;
        private Func<object, double> evaluator;

        public NumericPropertyExpression(IObjectExpression containingObject, PropertyInfo propertyInfo)
        {
            this.containingObject = containingObject;
            this.propertyInfo = propertyInfo;

            Type containingObjectType = containingObject.ObjectType;
            Type propertyType = propertyInfo.PropertyType;

            MethodInfo generateEvaluatorMethod = GetType()
                .GetMethod("GenerateEvaluator", BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(containingObjectType, propertyType);

            Func<PropertyInfo, Func<object, double>> generateEvaluatorDelegate =
                ReflectionHelper.CreateDelegate<PropertyInfo, Func<object, double>>(generateEvaluatorMethod);

            evaluator = generateEvaluatorDelegate(propertyInfo);
        }

        public double Evaluate(Dictionary<string, object> variables)
        {
            object instance = containingObject.GetInstance(variables);
            return evaluator(instance);
        }

        private static Func<object, double> GenerateEvaluator<T, TP>(PropertyInfo propertyInfo)
        {
            Func<T, TP> getter = ReflectionHelper.CreateDelegate<T, TP>(propertyInfo);
            Func<TP, double> converter = ReflectionHelper.GenerateConverter<TP>();

            return obj => converter(getter((T)obj));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(containingObject).Append('.').Append(propertyInfo.Name);
            return sb.ToString();
        }
    }
}
