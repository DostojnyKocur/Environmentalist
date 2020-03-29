using System;
using Environmentalist.Validators.ObjectValidator;
using NUnit.Framework;

namespace EnvironmentalistTests.Validators
{
    [TestFixture]
    public class ObjectValidatorTests
    {
        private IObjectValidator _sut;


        [SetUp]
        public void Init()
        {
            _sut = new ObjectValidator();
        }

        [Test]
        public void When_is_null_And_object_is_not_null_Then_does_not_throw_any_exception()
        {
            Assert.DoesNotThrow(() => _sut.IsNull(new object(), string.Empty));
        }

        [Test]
        public void When_is_null_And_object_is_null_Then_throws_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.IsNull(null, string.Empty));
        }
    }
}