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
    public class NumericFuncExpression : INumericExpression
    {
        private string functionName;
        private ListExpression argumentsExpr;
        private Func<Dictionary<string, object>, double> evaluator;

        public NumericFuncExpression(string functionName, Delegate function, ListExpression argumentsExpr)
        {
            this.functionName = functionName;
            this.argumentsExpr = argumentsExpr;

            MethodInfo methodInfo = function.Method;
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
                .CreateDelegate<Delegate, ListExpression, Func<Dictionary<string, object>, double>>(generateEvaluatorMethod);

            evaluator = generateEvaluatorDelegate(function, argumentsExpr);
        }

        public double Evaluate(Dictionary<string, object> variables)
        {
            return evaluator(variables);
        }

        private static Func<Dictionary<string, object>, double> GenerateFunctor0<T>(Delegate function, ListExpression argumentsExpr)
        {
            Func<T> invoker = (Func<T>)function;
            Func<T, double> converter = ReflectionHelper.GenerateConverter<T>();

            return variables => converter(invoker());
        }

        private static Func<Dictionary<string, object>, double> GenerateFunctor1<TP1, T>(Delegate function, ListExpression argumentsExpr)
        {
            Func<TP1, T> invoker = (Func<TP1, T>)function;
            Func<T, double> converter = ReflectionHelper.GenerateConverter<T>();
            Func<Dictionary<string, object>, TP1> argGetter1 = argumentsExpr.GetItemGetter<TP1>(0);

            return variables => converter(invoker(argGetter1(variables)));
        }

        private static Func<Dictionary<string, object>, double> GenerateFunctor2<TP1, TP2, T>(Delegate function, ListExpression argumentsExpr)
        {
            Func<TP1, TP2, T> invoker = (Func<TP1, TP2, T>)function;
            Func<T, double> converter = ReflectionHelper.GenerateConverter<T>();
            Func<Dictionary<string, object>, TP1> argGetter1 = argumentsExpr.GetItemGetter<TP1>(0);
            Func<Dictionary<string, object>, TP2> argGetter2 = argumentsExpr.GetItemGetter<TP2>(1);

            return variables => converter(invoker(argGetter1(variables), argGetter2(variables)));
        }

        private static Func<Dictionary<string, object>, double> GenerateFunctor3<TP1, TP2, TP3, T>(Delegate function, ListExpression argumentsExpr)
        {
            Func<TP1, TP2, TP3, T> invoker = (Func<TP1, TP2, TP3, T>)function;
            Func<T, double> converter = ReflectionHelper.GenerateConverter<T>();
            Func<Dictionary<string, object>, TP1> argGetter1 = argumentsExpr.GetItemGetter<TP1>(0);
            Func<Dictionary<string, object>, TP2> argGetter2 = argumentsExpr.GetItemGetter<TP2>(1);
            Func<Dictionary<string, object>, TP3> argGetter3 = argumentsExpr.GetItemGetter<TP3>(2);

            return variables => converter(invoker(argGetter1(variables), argGetter2(variables), argGetter3(variables)));
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", functionName, argumentsExpr);
        }
    }
}
