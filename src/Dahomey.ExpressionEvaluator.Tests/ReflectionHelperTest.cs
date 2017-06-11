#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

 using System;
using Xunit;

namespace Dahomey.ExpressionEvaluator.Tests
{
    public class ReflectionHelperTest
    {
        enum EnumTest
        {
            Value = 12
        }

        [Fact]
        public void GenerateEnumToDoubleConverter()
        {
            Func<EnumTest, double> converter = ReflectionHelper.GenerateConverter<EnumTest>();
            double value = converter(EnumTest.Value);
            Assert.Equal(12.0, value, 2);
        }

        [Fact]
        public void GenerateDoubleToEnumConverter()
        {
            Func<double, EnumTest> converter = ReflectionHelper.GenerateFromDoubleConverter<EnumTest>();
            EnumTest value = converter(12);
            Assert.Equal(EnumTest.Value, value);
        }
    }
}
