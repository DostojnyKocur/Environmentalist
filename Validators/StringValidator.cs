using System;

namespace Environmentalist.Validators
{
    internal static class StringValidator
    {
        public static void IsNullOrWhitespace(string @string, string paramName)
        {
            if(string.IsNullOrWhiteSpace(@string))
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
