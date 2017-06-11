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
    public class ObjectPropertyExpression : IObjectExpression
    {
        private IObjectExpression containingObject;
        private PropertyInfo propertyInfo;
        private readonly Func<object, object> evaluator;

        public Type ObjectType { get; private set; }

        public ObjectPropertyExpression(IObjectExpression containingObject, PropertyInfo propertyInfo)
        {
            this.containingObject = containingObject;
            this.propertyInfo = propertyInfo;

            Type containingObjectType = containingObject.ObjectType;
            ObjectType = propertyInfo.PropertyType;

            MethodInfo generateEvaluatorMethod = GetType()
                .GetMethod("GenerateEvaluator", BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(containingObjectType, ObjectType);

            Func<PropertyInfo, Func<object, object>> generateEvaluatorDelegate =
                ReflectionHelper.CreateDelegate<PropertyInfo, Func<object, object>>(generateEvaluatorMethod);

            evaluator = generateEvaluatorDelegate(propertyInfo);
        }

        public object GetInstance(Dictionary<string, object> variables)
        {
            object containingObjectInstance = containingObject.GetInstance(variables);
            return evaluator(containingObjectInstance);
        }

        private static Func<object, object> GenerateEvaluator<T, TP>(PropertyInfo propertyInfo)
        {
            Func<T, TP> propertyGetter = ReflectionHelper.CreateDelegate<T, TP>(propertyInfo);
            return obj => propertyGetter((T)obj);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", containingObject, propertyInfo.Name);
        }
    }
}
