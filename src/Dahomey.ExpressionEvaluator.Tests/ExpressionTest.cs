#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

 using System.Collections.Generic;
using Xunit;

namespace Dahomey.ExpressionEvaluator.Tests
{
    public class A
    {
        public B[] Bs { get; set; }
        public List<int> Ints { get; set; }
    }

    public class B
    {
        public int Id { get; set; }
    }

    public class ExpressionTest
    {
        [Fact]
        public void ObjectListElementExpression()
        {
            var variableObjectExpr = new ObjectVariableExpression("a", typeof(A));
            var listExpr = new ObjectPropertyExpression(variableObjectExpr, typeof(A).GetProperty("Bs"));
            var indexExpr = new NumericLiteralExpression(1);
            var expr = new ObjectListElementExpression(listExpr, indexExpr);

            B b;
            A a = new A
            {
                Bs = new[]
                {
                    new B { Id = 0 },
                    b = new B { Id = 1 },
                }
            };

            object instance = expr.GetInstance(new Dictionary<string, object> { { "a", a } });
            Assert.Same(b, instance);
        }

        [Fact]
        public void NumberListElementExpression()
        {
            var variableObjectExpr = new ObjectVariableExpression("a", typeof(A));
            var listExpr = new ObjectPropertyExpression(variableObjectExpr, typeof(A).GetProperty("Ints"));
            var indexExpr = new NumericLiteralExpression(1);
            var expr = new NumericListElementExpression(listExpr, indexExpr);

            A a = new A
            {
                Ints = new List<int> { 1, 2 }
            };

            double value = expr.Evaluate(new Dictionary<string, object> { { "a", a } });
            Assert.Equal(2.0, value);
        }
    }
}
