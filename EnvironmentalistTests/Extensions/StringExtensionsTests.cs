using System;
using Environmentalist.Extensions;
using NUnit.Framework;

namespace EnvironmentalistTests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [TestCase("(aaa)", "aaa")]
        [TestCase("b(aaa)", "aaa")]
        [TestCase("(aaa)b", "aaa")]
        [TestCase("b(aaa)b", "aaa")]
        [TestCase("()", "")]
        public void When_get_between_parentheses_Then_returns_correct_result(string testString, string expectedValue)
        {
            var result = testString.GetBetweenParentheses();

            Assert.AreEqual(expectedValue, result);
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase("(a")]
        [TestCase("a)")]
        [TestCase(")aaa(")]
        [TestCase("(aaa)(aaa)")]
        public void When_get_between_parentheses_And_invalid_string_Then_throws_argument_exception(string testString)
        {
            Assert.Throws<ArgumentException>(() => testString.GetBetweenParentheses());
        }
    }
}
