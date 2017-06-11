#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

 using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Dahomey.ExpressionEvaluator.Tests
{
    public class ExpressionParserTest
    {
        class A
        {
            public B B { get; set; }
            public List<B> Bs { get; set; }
            public int f(byte i, string str, bool b) { return 12 + i; }
            public B g(short i, string str, bool b) { return Bs[i]; }
        }

        class B
        {
            public int Id { get; set; }
        }

        public enum Color
        {
            Red = 1,
            Green = 2,
            Blue = 3
        }

        [Theory]
        [InlineData("1+2*3", "1 2 3 * +", 7)]
        [InlineData("(1+2)*-3", "1 2 + 3 - *", (1 + 2) * -3)]
        [InlineData("(1+2)*(3+4)", "1 2 + 3 4 + *", (1 + 2) * (3 + 4))]
        [InlineData("1+2+3", "1 2 + 3 +", 1 + 2 + 3)]
        [InlineData("1+2", "1 2 +", 1 + 2)]
        [InlineData("1 + Color.Green)", "1 Color.Green +", 1 + Color.Green)]
        public void NumericExpressionTest(string expression, string expectedRpn, double expectedValue)
        {
            ExpressionParser parser = new ExpressionParser();
            parser.RegisterAssembly(GetType().Assembly);
            INumericExpression expr = parser.ParseNumericExpression(expression);
            Assert.NotNull(expr);
            Assert.Equal(expectedRpn, expr.ToString());
            Assert.Equal(expectedValue, expr.Evaluate());
        }

        [Theory]
        [InlineData("(1+2) < (3+4)", "1 2 + 3 4 + <", true)]
        [InlineData("(1+2) <= (3+4)", "1 2 + 3 4 + <=", true)]
        [InlineData("(1+2) == (3+4)", "1 2 + 3 4 + ==", false)]
        [InlineData("true", "true", true)]
        [InlineData("false", "false", false)]
        [InlineData("1 < 2 || 1 > 2", "1 2 < 1 2 > ||", true)]
        [InlineData("1 < 2 && 1 > 2", "1 2 < 1 2 > &&", false)]
        [InlineData("!(1 > 2)", "1 2 > !", true)]
        public void BooleanExpressionTest(string expression, string expectedRpn, bool expectedValue)
        {
            IBooleanExpression expr = new ExpressionParser().ParseBooleanExpression(expression);
            Assert.NotNull(expr);
            Assert.Equal(expectedRpn, expr.ToString());
            Assert.Equal(expectedValue, expr.Evaluate());
        }

        [Fact]
        public void NumericVariableTest()
        {
            ExpressionParser parser = new ExpressionParser();
            parser.RegisterVariable<int>("a");

            int a = 2;
            INumericExpression expr = parser.ParseNumericExpression("1 + a");

            Assert.NotNull(expr);
            Assert.Equal("1 a +", expr.ToString());
            Assert.Equal(1 + a, expr.Evaluate(new Dictionary<string, object> { { "a", a } }));
        }

        [Fact]
        public void NumericListVariableTest()
        {
            ExpressionParser parser = new ExpressionParser();
            parser.RegisterVariable<List<int>>("a");

            List<int> a = new List<int> { 1, 2 };
            INumericExpression expr = parser.ParseNumericExpression("1 + a[1]");

            Assert.NotNull(expr);
            Assert.Equal("1 a[1] +", expr.ToString());
            Assert.Equal(1 + a[1], expr.Evaluate(new Dictionary<string, object> { { "a", a } }));
        }

        [Fact]
        public void NumericArrayVariableTest()
        {
            ExpressionParser parser = new ExpressionParser();
            parser.RegisterVariable<int[]>("a");

            int[] a = new[] { 1, 2 };
            INumericExpression expr = parser.ParseNumericExpression("1 + a[1]");

            Assert.NotNull(expr);
            Assert.Equal("1 a[1] +", expr.ToString());
            Assert.Equal(1 + a[1], expr.Evaluate(new Dictionary<string, object> { { "a", a } }));
        }

        [Fact]
        public void MemberAccessExpressionTest()
        {
            ExpressionParser parser = new ExpressionParser();
            parser.RegisterVariable<A>("a");

            INumericExpression expr = parser.ParseNumericExpression("1 + a.B.Id");

            Assert.NotNull(expr);
            Assert.Equal("1 a.B.Id +", expr.ToString());

            A a = new A { B = new B { Id = 12 } };
            Assert.Equal(13, expr.Evaluate(new Dictionary<string, object> { { "a", a } }));
        }

        [Fact]
        public void NumericFunctionTest()
        {
            Func<double, double> func = n => Math.Cos(n);
            ExpressionParser parser = new ExpressionParser();
            parser.RegisterFunction("cos", func);

            INumericExpression expr = parser.ParseNumericExpression("1 + cos(12)");

            Assert.NotNull(expr);
            Assert.Equal("1 cos(12) +", expr.ToString());
            Assert.Equal(1 + Math.Cos(12), expr.Evaluate());
        }

        [Fact]
        public void ObjectFunctionTest()
        {
            B[] items = new[]
            {
                new B { Id = 12 },
                new B { Id = 13 }
            };

            Func<int, B> func = n => items[n];
            ExpressionParser parser = new ExpressionParser();
            parser.RegisterFunction("item", func);

            INumericExpression expr = parser.ParseNumericExpression("1 + item(1).Id");

            Assert.NotNull(expr);
            Assert.Equal("1 item(1).Id +", expr.ToString());
            Assert.Equal(1 + items[1].Id, expr.Evaluate());
        }

        [Fact]
        public void NumericMethodFunctionTest()
        {
            ExpressionParser parser = new ExpressionParser();
            parser.RegisterVariable("a", typeof(A));

            INumericExpression expr = parser.ParseNumericExpression("1 + a.f(2,\"foo\",true)");

            Assert.NotNull(expr);
            Assert.Equal("1 a.f(2,\"foo\",true) +", expr.ToString());

            A a = new A();
            Assert.Equal(1 + a.f(2, "foo", true), expr.Evaluate(new Dictionary<string, object> { { "a", a } }));
        }

        [Fact]
        public void ObjectMethodFunctionTest()
        {
            ExpressionParser parser = new ExpressionParser();
            parser.RegisterVariable("a", typeof(A));

            INumericExpression expr = parser.ParseNumericExpression("1 + a.g(2,\"foo\",true).Id");

            Assert.NotNull(expr);
            Assert.Equal("1 a.g(2,\"foo\",true).Id +", expr.ToString());

            A a = new A
            {
                Bs = new List<B>
                {
                    new B { Id = 1 },
                    new B { Id = 2 },
                    new B { Id = 3 },
                }
            };
            Assert.Equal(1 + a.g(2, "foo", true).Id, expr.Evaluate(new Dictionary<string, object> { { "a", a } }));
        }

        public class Item
        {
            public Color Color { get; set; }
        }

        [Fact]
        public void EnumFuncParam()
        {
            ExpressionParser parser = new ExpressionParser();
            parser.RegisterAssembly(GetType().GetTypeInfo().Assembly);

            Func<Color, Color> colorFunc = c => c;
            parser.RegisterFunction("color", colorFunc);

            INumericExpression expr = parser.ParseNumericExpression("color(Color.Green)");
            Assert.NotNull(expr);
            Assert.Equal("color(Color.Green)", expr.ToString());
            Assert.Equal((int)Color.Green, expr.Evaluate());
        }

        [Fact]
        public void AdvancedEnumTest()
        {
            ExpressionParser parser = new ExpressionParser();
            parser.RegisterAssembly(GetType().GetTypeInfo().Assembly);

            List<Item> items = new List<Item>
            {
                new Item { Color = Color.Red },
                new Item { Color = Color.Green },
                new Item { Color = Color.Blue },
            };

            Func<int, Item> itemFunc = i => items[i];
            parser.RegisterFunction("item", itemFunc);

            IBooleanExpression expr = parser.ParseBooleanExpression("item(1).Color == Color.Green");
            Assert.NotNull(expr);
            Assert.Equal("item(1).Color Color.Green ==", expr.ToString());
            Assert.True(expr.Evaluate());
        }
    }
}
