using System;
using System.Collections.Generic;
using Environmentalist.Services.EnvironmentVariableReader;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;
using Moq;
using NUnit.Framework;

namespace EnvironmentalistTests.Services.EnvironmentVariableReader
{
    [TestFixture]
    class EnvironmentVariableReaderTests
    {
        private const string EnvironmentVariableName1 = nameof(EnvironmentVariableName1);
        private const string EnvironmentVariableName2 = nameof(EnvironmentVariableName2);
        private const string EnvironmentVariableValue1 = nameof(EnvironmentVariableValue1);
        private const string EnvironmentVariableValue2 = nameof(EnvironmentVariableValue2);

        private Mock<IStringValidator> _stringValidatorMock;
        private Mock<IObjectValidator> _objectValidator;
        private IEnvironmentVariableReader _sut;

        [SetUp]
        public void Init()
        {
            _stringValidatorMock = new Mock<IStringValidator>();
            _objectValidator = new Mock<IObjectValidator>();

            _sut = new Environmentalist.Services.EnvironmentVariableReader.EnvironmentVariableReader(
                _stringValidatorMock.Object,
                _objectValidator.Object);
        }

        [Test]
        public void When_read_Then_returns_environment_varible_value()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableName1, EnvironmentVariableValue1);

            var environmentVariableValue = _sut.Read(EnvironmentVariableName1);

            Assert.AreEqual(EnvironmentVariableValue1, environmentVariableValue);
        }

        [Test]
        public void When_read_And_environemnt_variable_is_null_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _sut.Read((string)null));
        }

        [Test]
        public void When_read_And_argument_is_list_Then_returns_environment_varible_values()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableName1, EnvironmentVariableValue1);
            Environment.SetEnvironmentVariable(EnvironmentVariableName2, EnvironmentVariableValue2);

            var environmentVariablesNames = new string[]{ EnvironmentVariableName1, EnvironmentVariableName2 };

            var environmentVariableValues = _sut.Read(environmentVariablesNames);

            Assert.NotNull(environmentVariableValues);
            Assert.AreEqual(EnvironmentVariableValue1, environmentVariableValues[EnvironmentVariableName1]);
            Assert.AreEqual(EnvironmentVariableValue2, environmentVariableValues[EnvironmentVariableName2]);
        }

        [Test]
        public void When_read_And_environemnt_variable__list_is_null_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _sut.Read((IEnumerable<string>)null));
        }
    }
}
