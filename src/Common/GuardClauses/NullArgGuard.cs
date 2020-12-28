using System;

namespace Ardalis.GuardClauses
{
    public static class NullArgGuard
    {
        public static void NullArg<T>(this IGuardClause guardClause, T input, string parameterName) where T: class
        {
            if (input == null)
            {
                throw new ArgumentNullException("Should not be null!", parameterName);
            }
        }
    }
}