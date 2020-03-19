using System;
using Environmentalist.Validators.StringValidator;
using NUnit.Framework;

namespace EnvironmentalistTests.Validators
{
    public class StringValidatorTests
    {
        private IStringValidator _sut;


        [SetUp]
        public void Init()
        {
            _sut = new StringValidator();
        }

        [Test]
        public void When_is_null_or_whitespace_And_string_is_valid_Then_does_not_throw_any_exception()
        {
            Assert.DoesNotThrow(() => _sut.IsNullOrWhitespace("str", string.Empty));
        }

        [Test]
        public void When_is_null_or_whitespace_And_string_is_null_Then_throws_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.IsNullOrWhitespace(null, string.Empty));
        }

        [Test]
        public void When_is_null_or_whitespace_And_string_is_empty_Then_throws_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.IsNullOrWhitespace(string.Empty, string.Empty));
        }

        [Test]
        public void When_is_null_or_whitespace_And_string_is_whitespace_Then_throws_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.IsNullOrWhitespace("   ", string.Empty));
        }
    }
}
