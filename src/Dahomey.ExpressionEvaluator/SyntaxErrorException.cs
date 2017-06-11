#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

 using System;

namespace Dahomey.ExpressionEvaluator
{
    public class SyntaxErrorException : Exception
    {
        public SyntaxErrorException()
        {
        }

        public SyntaxErrorException(string message) : base(message)
        {
        }

        public SyntaxErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}