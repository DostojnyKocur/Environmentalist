using System;

namespace Environmentalist.Extensions
{
    public static class StringExtensions
    {
        public static string GetBetweenParentheses(this string @string)
        {
            try
            {
                var result = @string.Split('(', ')');
                if(result.Length != 3 || //Case when there is no '(' or ')' or and more parentheses
                   (@string.IndexOf('(') > @string.IndexOf(')')) || //Case when first occurs ')' and then '('
                   result[1] is null) 
                {
                    throw new ArgumentException($"Cannot retrieve value between '(' and ')' from '{@string}'");
                }

                return result[1];
            }
            catch
            {
                throw new ArgumentException($"Cannot retrieve value between '(' and ')' from '{@string}'");
            }
        }
    }
}
