#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

 using System.Collections.Generic;

namespace Dahomey.ExpressionEvaluator
{
    public interface IBooleanExpression : IExpression
    {
        bool Evaluate(Dictionary<string, object> variables = null);
    }
}
