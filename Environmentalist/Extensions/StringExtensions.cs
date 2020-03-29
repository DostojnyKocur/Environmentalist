namespace Environmentalist.Extensions
{
    public static class StringExtensions
    {
        public static string GetBetweenParentheses(this string @string)
        {
            return @string.Split('(', ')')[1];
        }
    }
}
