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
    public class ExpressionParser
    {
        private ExpressionLexer lexer;
        private Dictionary<string, Type> variableTypes = new Dictionary<string, Type>();
        private Dictionary<string, Delegate> functions = new Dictionary<string, Delegate>();
        private List<Assembly> assemblies = new List<Assembly>();

        public void RegisterVariable(string variableName, Type variableType)
        {
            variableTypes[variableName] = variableType;
        }

        public void RegisterVariable<T>(string variableName)
        {
            variableTypes[variableName] = typeof(T);
        }

        public void RegisterFunction(string functionName, Delegate function)
        {
            functions[functionName] = function;
        }

        public void RegisterAssembly(Assembly assembly)
        {
            assemblies.Add(assembly);
        }

        public IBooleanExpression ParseBooleanExpression(string expression)
        {
            lexer = new ExpressionLexer(expression);
            return (IBooleanExpression)Expression();
        }

        public INumericExpression ParseNumericExpression(string expression)
        {
            lexer = new ExpressionLexer(expression);
            return (INumericExpression)Expression();
        }

        private IExpression Expression()
        {
            return ConditionalExpression();
        }

        private IExpression ConditionalExpression()
        {
            IExpression expr = LogicalOrExpression();

            if (lexer.Accept(TokenType.Interrogation))
            {
                IExpression leftExpr = Expression();
                lexer.Expect(TokenType.Colon);
                IExpression rightExpr = Expression();

                return new NumericConditionalExpression
                {
                    ConditionExpr = (IBooleanExpression)expr,
                    LeftExpr = (INumericExpression)leftExpr,
                    RightExpr = (INumericExpression)rightExpr,
                };
            }

            return expr;
        }

        private IExpression LogicalOrExpression()
        {
            IExpression expr = LogicalAndExpression();

            while (lexer.Accept(TokenType.Or))
            {
                expr = new BooleanLogicalExpression
                {
                    Operator = Operator.Or,
                    LeftExpr = (IBooleanExpression)expr,
                    RightExpr = (IBooleanExpression)LogicalAndExpression()
                };
            }

            return expr;
        }

        private IExpression LogicalAndExpression()
        {
            IExpression expr = BitwiseOrExpression();

            while (lexer.Accept(TokenType.And))
            {
                expr = new BooleanLogicalExpression
                {
                    Operator = Operator.And,
                    LeftExpr = (IBooleanExpression)expr,
                    RightExpr = (IBooleanExpression)BitwiseOrExpression()
                };
            }

            return expr;
        }

        private IExpression BitwiseOrExpression()
        {
            IExpression expr = BitwiseXorExpression();

            while (lexer.Accept(TokenType.BitwiseOr))
            {
                expr = new NumericArithmeticExpression
                {
                    Operator = Operator.BitwiseOr,
                    LeftExpr = (INumericExpression)expr,
                    RightExpr = (INumericExpression)BitwiseXorExpression()
                };
            }

            return expr;
        }

        private IExpression BitwiseXorExpression()
        {
            IExpression expr = BitwiseAndExpression();

            while (lexer.Accept(TokenType.BitwiseXor))
            {
                expr = new NumericArithmeticExpression
                {
                    Operator = Operator.BitwiseXor,
                    LeftExpr = (INumericExpression)expr,
                    RightExpr = (INumericExpression)BitwiseAndExpression()
                };
            }

            return expr;
        }

        private IExpression BitwiseAndExpression()
        {
            IExpression expr = EqualityExpression();

            while (lexer.Accept(TokenType.BitwiseAnd))
            {
                expr = new NumericArithmeticExpression
                {
                    Operator = Operator.BitwiseAnd,
                    LeftExpr = (INumericExpression)expr,
                    RightExpr = (INumericExpression)EqualityExpression()
                };
            }

            return expr;
        }

        private IExpression EqualityExpression()
        {
            IExpression expr = RelationalExpression();

            Operator op;
            if (lexer.Accept(TokenType.Eq))
            {
                op = Operator.Equal;
            }
            else if (lexer.Accept(TokenType.Ne))
            {
                op = Operator.NotEqual;
            }
            else
            {
                return expr;
            }

            return new NumericComparisonExpression
            {
                Operator = op,
                LeftExpr = (INumericExpression)expr,
                RightExpr = (INumericExpression)RelationalExpression()
            };
        }

        private IExpression RelationalExpression()
        {
            IExpression expr = ShiftExpression();

            Operator op;
            if (lexer.Accept(TokenType.Lt))
            {
                op = Operator.LessThan;
            }
            else if (lexer.Accept(TokenType.Le))
            {
                op = Operator.LessThanOrEqual;
            }
            else if (lexer.Accept(TokenType.Ge))
            {
                op = Operator.GreaterThanOrEqual;
            }
            else if (lexer.Accept(TokenType.Gt))
            {
                op = Operator.GreaterThan;
            }
            else
            {
                return expr;
            }

            return new NumericComparisonExpression
            {
                Operator = op,
                LeftExpr = (INumericExpression)expr,
                RightExpr = (INumericExpression)ShiftExpression()
            };
        }

        private IExpression ShiftExpression()
        {
            IExpression expr = AdditiveExpression();

            while (!lexer.Peek(TokenType.None))
            {
                Operator op;
                if (lexer.Accept(TokenType.LeftShift))
                {
                    op = Operator.LeftShift;
                }
                else if (lexer.Accept(TokenType.RightShift))
                {
                    op = Operator.RightShift;
                }
                else
                {
                    break;
                }

                expr = new NumericArithmeticExpression
                {
                    Operator = op,
                    LeftExpr = (INumericExpression)expr,
                    RightExpr = (INumericExpression)AdditiveExpression()
                };
            }

            return expr;
        }

        private IExpression AdditiveExpression()
        {
            IExpression expr = MultiplicativeExpression();

            while (!lexer.Peek(TokenType.None))
            {
                Operator op;
                if (lexer.Accept(TokenType.Plus))
                {
                    op = Operator.Plus;
                }
                else if (lexer.Accept(TokenType.Minus))
                {
                    op = Operator.Minus;
                }
                else
                {
                    break;
                }

                expr = new NumericArithmeticExpression
                {
                    Operator = op,
                    LeftExpr = (INumericExpression)expr,
                    RightExpr = (INumericExpression)MultiplicativeExpression()
                };
            }

            return expr;
        }

        private IExpression MultiplicativeExpression()
        {
            IExpression expr = UnaryExpression();

            while (!lexer.Peek(TokenType.None))
            {
                Operator op;
                if (lexer.Accept(TokenType.Mult))
                {
                    op = Operator.Mult;
                }
                else if (lexer.Accept(TokenType.Div))
                {
                    op = Operator.Div;
                }
                else if (lexer.Accept(TokenType.Mod))
                {
                    op = Operator.Mod;
                }
                else
                {
                    break;
                }

                expr = new NumericArithmeticExpression
                {
                    Operator = op,
                    LeftExpr = (INumericExpression)expr,
                    RightExpr = (INumericExpression)UnaryExpression()
                };
            }

            return expr;
        }

        private IExpression UnaryExpression()
        {
            if (lexer.Accept(TokenType.Minus))
            {
                return new NumericArithmeticExpression
                {
                    Operator = Operator.Minus,
                    LeftExpr = (INumericExpression)PrimaryExpression()
                };
            }
            else if (lexer.Accept(TokenType.BitwiseComplement))
            {
                return new NumericArithmeticExpression
                {
                    Operator = Operator.BitwiseComplement,
                    LeftExpr = (INumericExpression)PrimaryExpression()
                };
            }
            else if (lexer.Accept(TokenType.Not))
            {
                return new BooleanLogicalExpression
                {
                    Operator = Operator.Not,
                    LeftExpr = (IBooleanExpression)PrimaryExpression()
                };
            }
            else
            {
                return PrimaryExpression();
            }
        }

        private IExpression PrimaryExpression()
        {
            if (lexer.Peek(TokenType.OpenParenthesis))
            {
                return ParenthesizedExpression();
            }
            else if (lexer.Peek(TokenType.Identifier))
            {
                return VariableExpression();
            }

            return Literal();
        }

        private IExpression VariableExpression()
        {
            IExpression expr = ElementExpression(VariableOrFunctionExpression());

            while (lexer.Accept(TokenType.Dot))
            {
                IObjectExpression containingObjectExpr = expr as IObjectExpression;

                if (containingObjectExpr == null)
                {
                    throw BuildException("Cannot access property or method on expression {0}", expr);
                }

                expr = ElementExpression(MemberExpression(containingObjectExpr));
            }

            return expr;
        }

        private IExpression ElementExpression(IExpression expr)
        {
            if (lexer.Peek(TokenType.OpenBracket))
            {
                INumericExpression indexExpr = (INumericExpression)BracketExpression();
                IObjectExpression listExpr = expr as IObjectExpression;

                if (listExpr == null)
                {
                    throw BuildException("Cannot apply indexing with [] on expression", expr);
                }

                if (ReflectionHelper.IsNumberList(listExpr.ObjectType))
                {
                    expr = new NumericListElementExpression(listExpr, indexExpr);
                }
                else
                {
                    expr = new ObjectListElementExpression(listExpr, indexExpr);
                }
            }

            return expr;
        }

        private IExpression VariableOrFunctionExpression()
        {
            string identifier = lexer.Identifier();

            // function
            if (lexer.Peek(TokenType.OpenParenthesis))
            {
                ListExpression argsExpr = (ListExpression)InvocationExpression();

                Delegate function;

                if (!functions.TryGetValue(identifier, out function))
                {
                    throw BuildException("Unknown function '{0}()'", identifier);
                }

                MethodInfo methodInfo = function.Method;

                if (ReflectionHelper.IsNumber(methodInfo.ReturnType))
                {
                    return new NumericFuncExpression(identifier, function, argsExpr);
                }
                else
                {
                    return new ObjectFuncExpression(identifier, function, argsExpr);
                }
            }
            // variable or enum
            else
            {
                Type identifierType;
                // variable
                if (variableTypes.TryGetValue(identifier, out identifierType))
                {
                    if (ReflectionHelper.IsNumber(identifierType))
                    {
                        return new NumericVariableExpression(identifier, identifierType);
                    }
                    else
                    {
                        return new ObjectVariableExpression(identifier, identifierType);
                    }
                }
                else
                {
                    identifierType = ReflectionHelper.GetType(assemblies, identifier);
                    // enum

                    if (identifier != null && identifierType.IsEnum)
                    {
                        lexer.Expect(TokenType.Dot);
                        string enumValue = lexer.Identifier();

                        Enum value = (Enum)Enum.Parse(identifierType, enumValue);
                        return new EnumLiteralExpression(value);
                    }
                    else
                    {
                        throw BuildException(string.Format("Unknown variable '{0}'", identifier));
                    }
                }
            }
        }

        private IExpression MemberExpression(IObjectExpression containingObjectExpr)
        {
            string identifier = lexer.Identifier();

            // method
            if (lexer.Peek(TokenType.OpenParenthesis))
            {
                ListExpression argsExpr = (ListExpression)InvocationExpression();

                MethodInfo methodInfo = containingObjectExpr.ObjectType.GetMethod(identifier);
                Type returnType = methodInfo.ReturnType;

                if (ReflectionHelper.IsNumber(returnType))
                {
                    return new NumericMethodExpression(containingObjectExpr, methodInfo, argsExpr);
                }
                else
                {
                    return new ObjectMethodExpression(containingObjectExpr, methodInfo, argsExpr);
                }
            }
            // property
            else
            {
                PropertyInfo propertyInfo = containingObjectExpr.ObjectType.GetProperty(identifier);
                Type propertyType = propertyInfo.PropertyType;

                if (ReflectionHelper.IsNumber(propertyType))
                {
                    return new NumericPropertyExpression(containingObjectExpr, propertyInfo);
                }
                else
                {
                    return new ObjectPropertyExpression(containingObjectExpr, propertyInfo);
                }
            }
        }

        private IExpression Literal()
        {
            if (lexer.Accept(TokenType.True))
            {
                return new BooleanLiteralExpression(true);
            }

            if (lexer.Accept(TokenType.False))
            {
                return new BooleanLiteralExpression(false);
            }

            if (lexer.Peek(TokenType.Number))
            {
                return new NumericLiteralExpression(lexer.Number());
            }

            if (lexer.Peek(TokenType.String))
            {
                return new StringLiteralExpression(lexer.String());
            }

            throw BuildException("Expected boolean, number or string literal");
        }

        private IExpression ParenthesizedExpression()
        {
            lexer.Expect(TokenType.OpenParenthesis);
            IExpression expr = Expression();
            lexer.Expect(TokenType.CloseParenthesis);
            return expr;
        }

        private IExpression BracketExpression()
        {
            lexer.Expect(TokenType.OpenBracket);
            IExpression expr = Expression();
            lexer.Expect(TokenType.CloseBracket);
            return expr;
        }

        private IExpression InvocationExpression()
        {
            lexer.Expect(TokenType.OpenParenthesis);

            List<IExpression> args = new List<IExpression>();
            ListExpression argsExpr = new ListExpression(args);

            if (lexer.Accept(TokenType.CloseParenthesis))
            {
                return argsExpr;
            }

            do
            {
                args.Add(Expression());
            }
            while (lexer.Accept(TokenType.Comma));

            lexer.Expect(TokenType.CloseParenthesis);

            return argsExpr;
        }

        public Exception BuildException(string message)
        {
            return lexer.BuildException(message);
        }

        public Exception BuildException(string message, params object[] args)
        {
            return lexer.BuildException(message, args);
        }
    }
}
