#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

using System;

namespace Dahomey.ExpressionEvaluator
{
    public enum Operator
    {
        Mult, // *
        Div, // /
        Mod, // %
        Pow, // ^
        Minus, // -
        Plus, // +
        Cos, // Cos
        Sin, // Sin
        Tan, // Tan
        Abs, // Abs
        Sqrt, // Sqrt
        LeftShift, // <<
        RightShift, // >>
        LessThan, // <
        LessThanOrEqual, // <=
        GreaterThanOrEqual, // >=
        GreaterThan, // >
        Equal, // ==
        NotEqual, // !=
        BitwiseAnd, // &
        BitwiseXor, // ^^
        BitwiseOr, // |
        BitwiseComplement, // ~
        Not, // !
        And, // &&
        Or, // ||
    }

    public static class OperatorExtension
    {
        public static string PrettyPrint(this Operator op)
        {
            switch (op)
            {
                case Operator.Mult:
                    return "*";

                case Operator.Div:
                    return "/";

                case Operator.Mod:
                    return "%";

                case Operator.Minus:
                    return "-";
                
                case Operator.Pow:
                    return "^";

                case Operator.Plus:
                    return "+";
                
                case Operator.Sin:
                    return "Sin";
                
                case Operator.Cos:
                    return "Cos";
                
                case Operator.Abs:
                    return "Abs";
                
                case Operator.Tan:
                    return "Tan";
                
                case Operator.Sqrt:
                    return "Sqrt";
                
                case Operator.LeftShift:
                    return "<<";

                case Operator.RightShift:
                    return ">>";

                case Operator.LessThan:
                    return "<";

                case Operator.LessThanOrEqual:
                    return "<=";

                case Operator.GreaterThanOrEqual:
                    return ">=";

                case Operator.GreaterThan:
                    return ">";

                case Operator.Equal:
                    return "==";

                case Operator.NotEqual:
                    return "!=";

                case Operator.BitwiseAnd:
                    return "&";

                case Operator.BitwiseOr:
                    return "|";

                case Operator.BitwiseXor:
                    return "^^";

                case Operator.BitwiseComplement:
                    return "~";

                case Operator.And:
                    return "&&";

                case Operator.Or:
                    return "||";

                case Operator.Not:
                    return "!";

                default:
                    throw new NotSupportedException(string.Format("Operator {0} not supported", op));
            }
        }
    }
}
