#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

using Dahomey.ExpressionEvaluator.Expressions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dahomey.ExpressionEvaluator
{
    public class ObjectMethodExpression : IObjectExpression
    {
        private IObjectExpression containingObject;
        private MethodInfo methodInfo;
        private ListExpression argumentsExpr;
        private Func<Dictionary<string, object>, object, object> evaluator;

        public Type ObjectType { get; private set; }

        public ObjectMethodExpression(IObjectExpression containingObject, MethodInfo methodInfo, ListExpression argumentsExpr)
        {
            this.containingObject = containingObject;
            this.methodInfo = methodInfo;
            this.argumentsExpr = argumentsExpr;

            Type containingObjectType = containingObject.ObjectType;
            ObjectType = methodInfo.ReturnType;
            MethodInfo generateEvaluatorMethod;
            ParameterInfo[] parameters = methodInfo.GetParameters();

            switch (parameters.Length)
            {
                case 0:
                    generateEvaluatorMethod = GetType()
                        .GetMethod("GenerateFunctor0", BindingFlags.NonPublic | BindingFlags.Static)
                        .MakeGenericMethod(containingObjectType, ObjectType);
                    break;

                case 1:
                    generateEvaluatorMethod = GetType()
                        .GetMethod("GenerateFunctor1", BindingFlags.NonPublic | BindingFlags.Static)
                        .MakeGenericMethod(containingObjectType, parameters[0].ParameterType, ObjectType);
                    break;

                case 2:
                    generateEvaluatorMethod = GetType()
                        .GetMethod("GenerateFunctor2", BindingFlags.NonPublic | BindingFlags.Static)
                        .MakeGenericMethod(containingObjectType,
                            parameters[0].ParameterType,
                            parameters[1].ParameterType,
                            ObjectType);
                    break;

                case 3:
                    generateEvaluatorMethod = GetType()
                        .GetMethod("GenerateFunctor3", BindingFlags.NonPublic | BindingFlags.Static)
                        .MakeGenericMethod(containingObjectType,
                            parameters[0].ParameterType,
                            parameters[1].ParameterType,
                            parameters[2].ParameterType,
                            ObjectType);
                    break;

                default:
                    throw new NotSupportedException();
            }

            var generateEvaluatorDelegate = ReflectionHelper
                .CreateDelegate<MethodInfo, ListExpression, Func<Dictionary<string, object>, object, object>>(generateEvaluatorMethod);

            evaluator = generateEvaluatorDelegate(methodInfo, argumentsExpr);
        }

        public object GetInstance(Dictionary<string, object> variables)
        {
            object instance = containingObject.GetInstance(variables);
            return evaluator(variables, instance);
        }

        private static Func<Dictionary<string, object>, object, object> GenerateFunctor0<T, TM>(MethodInfo methodInfo, ListExpression argumentsExpr)
        {
            Func<T, TM> invoker = ReflectionHelper.CreateDelegate<T, TM>(methodInfo);

            return (variables, obj) => invoker((T)obj);
        }

        private static Func<Dictionary<string, object>, object, object> GenerateFunctor1<T, TP, TM>(MethodInfo methodInfo, ListExpression argumentsExpr)
        {
            Func<T, TP, TM> invoker = ReflectionHelper.CreateDelegate<T, TP, TM>(methodInfo);
            Func<Dictionary<string, object>, TP> argGetter = argumentsExpr.GetItemGetter<TP>(0);

            return (variables, obj) => invoker((T)obj, argGetter(variables));
        }

        private static Func<Dictionary<string, object>, object, object> GenerateFunctor2<T, TP1, TP2, TM>(MethodInfo methodInfo, ListExpression argumentsExpr)
        {
            Func<T, TP1, TP2, TM> invoker = ReflectionHelper.CreateDelegate<T, TP1, TP2, TM>(methodInfo);
            Func<Dictionary<string, object>, TP1> argGetter1 = argumentsExpr.GetItemGetter<TP1>(0);
            Func<Dictionary<string, object>, TP2> argGetter2 = argumentsExpr.GetItemGetter<TP2>(1);

            return (variables, obj) => invoker((T)obj, argGetter1(variables), argGetter2(variables));
        }

        private static Func<Dictionary<string, object>, object, object> GenerateFunctor3<T, TP1, TP2, TP3, TM>(MethodInfo methodInfo, ListExpression argumentsExpr)
        {
            Func<T, TP1, TP2, TP3, TM> invoker = ReflectionHelper.CreateDelegate<T, TP1, TP2, TP3, TM>(methodInfo);
            Func<Dictionary<string, object>, TP1> argGetter1 = argumentsExpr.GetItemGetter<TP1>(0);
            Func<Dictionary<string, object>, TP2> argGetter2 = argumentsExpr.GetItemGetter<TP2>(1);
            Func<Dictionary<string, object>, TP3> argGetter3 = argumentsExpr.GetItemGetter<TP3>(2);

            return (variables, obj) => invoker((T)obj, argGetter1(variables), argGetter2(variables), argGetter3(variables));
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}({2})", containingObject, methodInfo.Name, argumentsExpr);
        }
    }
}
