using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Environmentalist.Models;
using Environmentalist.Services.Readers.EnvironmentVariableReader;
using Environmentalist.Services.Readers.TemplateReader;
using Environmentalist.Services.Repositories.TemplateRepository;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;
using Moq;
using NUnit.Framework;

namespace EnvironmentalistTests.Services.Repositories.TemplateRepository
{
    [TestFixture]
    public class TemplateRepositoryTests
    {
        private const string Path = @"some path";
        private const string Field1 = nameof(Field1);
        private const string Field2 = nameof(Field2);
        private const string Value1 = nameof(Value1);
        private const string SecureVaultPathVarName = "keepass";

        private const string SecureVaultPathVarValue = nameof(SecureVaultPathVarValue);
        private const string SecureVaultPassVarValue = nameof(SecureVaultPassVarValue);

        private static readonly string SecureVaultPathVarLine = $"[EnvVar]({SecureVaultPathVarName})";

        private static readonly Dictionary<string, string> EnvVarValues = new Dictionary<string, string>
        {
            { SecureVaultPathVarName, SecureVaultPathVarValue }
        };

        private static TemplateModel Template;

        private Mock<ITemplateReader> _templateReaderMock;
        private Mock<IEnvironmentVariableReader> _environmentVariableReaderMock;
        private Mock<IStringValidator> _stringValidatorMock;
        private Mock<IObjectValidator> _objectValidatorMock;
        private ITemplateRepository _sut;

        [SetUp]
        public void Init()
        {
            Template = new TemplateModel();
            Template.Fields.Add(Field1, Value1);
            Template.Fields.Add(Field2, SecureVaultPathVarLine);

            _templateReaderMock = new Mock<ITemplateReader>();
            _environmentVariableReaderMock = new Mock<IEnvironmentVariableReader>();
            _stringValidatorMock = new Mock<IStringValidator>();
            _objectValidatorMock = new Mock<IObjectValidator>();

            _sut = new Environmentalist.Services.Repositories.TemplateRepository.TemplateRepository(
                _templateReaderMock.Object,
                _environmentVariableReaderMock.Object,
                _stringValidatorMock.Object,
                _objectValidatorMock.Object);
        }

        [Test]
        public async Task When_get_template_Then_returns_template()
        {
            _templateReaderMock.Setup(m => m.Read(Path)).ReturnsAsync(Template);
            _environmentVariableReaderMock.Setup(m => m.Read(It.IsAny<IEnumerable<string>>())).Returns(EnvVarValues);

            var result = await _sut.GetTemplate(Path);

            Assert.IsNotNull(result);
            Assert.AreEqual(Value1, result.Fields[Field1]);
            Assert.AreEqual(SecureVaultPathVarValue, result.Fields[Field2]);
        }

        [Test]
        public void When_get_template_And_path_is_invalid_then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetTemplate(null));
        }
    }
}
