using System;
using Environmentalist.Validators;
using NUnit.Framework;

namespace EnvironmentalistTests.Validators
{
    public class ObjectValidatorTests
    {
        [Test]
        public void When_is_null_And_object_is_not_null_Then_does_not_throw_any_exception()
        {
            Assert.DoesNotThrow(() => ObjectValidator.IsNull(new object(), string.Empty));
        }

        [Test]
        public void When_is_null_And_object_is_null_Then_throws_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => ObjectValidator.IsNull(null, string.Empty));
        }
    }
}