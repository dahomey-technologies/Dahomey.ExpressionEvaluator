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
        Minus, // -
        Plus, // +
        LeftShift, // <<
        RightShift, // >>
        LessThan, // <
        LessThanOrEqual, // <=
        GreaterThanOrEqual, // >=
        GreaterThan, // >
        Equal, // ==
        NotEqual, // !=
        BitwiseAnd, // &
        BitwiseXor, // ^
        BitwiseOr, // |
        BitwiseComplement, // ~
        Not, // !
        And, // &&
        Or, // ||
    }

    internal static class OperatorExtension
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

                case Operator.Plus:
                    return "+";

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
                    return "^";

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
