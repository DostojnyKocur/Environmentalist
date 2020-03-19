using System;

namespace Environmentalist.Validators.StringValidator
{
    public sealed class StringValidator : IStringValidator
    {
        public void IsNullOrWhitespace(string @string, string paramName)
        {
            if(string.IsNullOrWhiteSpace(@string))
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
