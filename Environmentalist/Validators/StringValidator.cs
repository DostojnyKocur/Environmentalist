using System;

namespace Environmentalist.Validators
{
    public class StringValidator
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
