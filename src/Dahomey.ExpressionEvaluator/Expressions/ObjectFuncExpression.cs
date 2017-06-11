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
    public class ObjectFuncExpression : IObjectExpression
    {
        private string functionName;
        private ListExpression argumentsExpr;
        private Func<Dictionary<string, object>, object> evaluator;
        public Type ObjectType { get; private set; }

        public ObjectFuncExpression(string functionName, Delegate function, ListExpression argumentsExpr)
        {
            this.functionName = functionName;
            this.functionName = functionName;
            this.argumentsExpr = argumentsExpr;

            MethodInfo methodInfo = function.Method;
            ObjectType = methodInfo.ReturnType;
            Type returnType = methodInfo.ReturnType;
            MethodInfo generateEvaluatorMethod;
            ParameterInfo[] parameters = methodInfo.GetParameters();

            switch (parameters.Length)
            {
                case 0:
                    generateEvaluatorMethod = GetType()
                        .GetMethod("GenerateFunctor0", BindingFlags.NonPublic | BindingFlags.Static)
                        .MakeGenericMethod(returnType);
                    break;

                case 1:
                    generateEvaluatorMethod = GetType()
                        .GetMethod("GenerateFunctor1", BindingFlags.NonPublic | BindingFlags.Static)
                        .MakeGenericMethod(parameters[0].ParameterType, returnType);
                    break;

                case 2:
                    generateEvaluatorMethod = GetType()
                        .GetMethod("GenerateFunctor2", BindingFlags.NonPublic | BindingFlags.Static)
                        .MakeGenericMethod(
                            parameters[0].ParameterType,
                            parameters[1].ParameterType,
                            returnType);
                    break;

                case 3:
                    generateEvaluatorMethod = GetType()
                        .GetMethod("GenerateFunctor3", BindingFlags.NonPublic | BindingFlags.Static)
                        .MakeGenericMethod(
                            parameters[0].ParameterType,
                            parameters[1].ParameterType,
                            parameters[2].ParameterType,
                            returnType);
                    break;

                default:
                    throw new NotSupportedException();
            }

            var generateEvaluatorDelegate = ReflectionHelper
                .CreateDelegate<Delegate, ListExpression, Func<Dictionary<string, object>, object>>(generateEvaluatorMethod);

            evaluator = generateEvaluatorDelegate(function, argumentsExpr);
        }

        public object GetInstance(Dictionary<string, object> variables)
        {
            return evaluator(variables);
        }

        private static Func<Dictionary<string, object>, object> GenerateFunctor0<T>(MethodInfo methodInfo, ListExpression argumentsExpr)
        {
            Func<T> invoker = ReflectionHelper.CreateDelegate<T>(methodInfo);

            return variables => invoker();
        }

        private static Func<Dictionary<string, object>, object> GenerateFunctor1<TP1, T>(Delegate function, ListExpression argumentsExpr)
        {
            Func<TP1, T> invoker = (Func<TP1, T>)function;
            Func<Dictionary<string, object>, TP1> argGetter1 = argumentsExpr.GetItemGetter<TP1>(0);

            return variables => invoker(argGetter1(variables));
        }

        private static Func<Dictionary<string, object>, object> GenerateFunctor2<TP1, TP2, T>(MethodInfo methodInfo, ListExpression argumentsExpr)
        {
            Func<TP1, TP2, T> invoker = ReflectionHelper.CreateDelegate<TP1, TP2, T>(methodInfo);
            Func<Dictionary<string, object>, TP1> argGetter1 = argumentsExpr.GetItemGetter<TP1>(0);
            Func<Dictionary<string, object>, TP2> argGetter2 = argumentsExpr.GetItemGetter<TP2>(1);

            return variables => invoker(argGetter1(variables), argGetter2(variables));
        }

        private static Func<Dictionary<string, object>, object> GenerateFunctor3<TP1, TP2, TP3, T>(MethodInfo methodInfo, ListExpression argumentsExpr)
        {
            Func<TP1, TP2, TP3, T> invoker = ReflectionHelper.CreateDelegate<TP1, TP2, TP3, T>(methodInfo);
            Func<Dictionary<string, object>, TP1> argGetter1 = argumentsExpr.GetItemGetter<TP1>(0);
            Func<Dictionary<string, object>, TP2> argGetter2 = argumentsExpr.GetItemGetter<TP2>(1);
            Func<Dictionary<string, object>, TP3> argGetter3 = argumentsExpr.GetItemGetter<TP3>(2);

            return variables => invoker(argGetter1(variables), argGetter2(variables), argGetter3(variables));
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", functionName, argumentsExpr);
        }
    }
}
