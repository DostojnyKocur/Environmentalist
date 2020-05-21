using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Environmentalist.Models;
using Environmentalist.Services.Readers.ConfigurationReader;
using Environmentalist.Services.Readers.EnvironmentVariableReader;
using Environmentalist.Services.Repositories.ConfigurationRepository;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;
using Moq;
using NUnit.Framework;

namespace EnvironmentalistTests.Services.Repositories.ConfigurationRepository
{
    [TestFixture]
    public class ConfigurationRepositoryTests
    {
        private const string Path = @"some path";
        private const string TemplatePath = "template.env";
        private const string ResultPath = "result.env";
        private const string ProfilePath = "profile.txt";
        private const string SecureVaultPathVarName = "keepass";
        private const string SecureVaultPassVarName = "pass";
        private const string ProtectedFilePath = "protected.txt";
        private const string ProtectedFileEntropy = "entropy==";

        private const string SecureVaultPathVarValue = nameof(SecureVaultPathVarValue);
        private const string SecureVaultPassVarValue = nameof(SecureVaultPassVarValue);

        private static readonly string SecureVaultPathVarLine = $"[EnvVar]({SecureVaultPathVarName})";
        private static readonly string SecureVaultPassVarLine = $"[EnvVar]({SecureVaultPassVarName})";

        private static readonly Dictionary<string, string> EnvVarValues = new Dictionary<string, string>
        {
            { SecureVaultPathVarName, SecureVaultPathVarValue },
            { SecureVaultPassVarName, SecureVaultPassVarValue },
        };

        private readonly ConfigurationModel Configuration = new ConfigurationModel
        {
            TemplatePath = TemplatePath,
            ResultPath = ResultPath,
            ProfilePath = ProfilePath,
            SecureVaultPath = SecureVaultPathVarLine,
            SecureVaultPass = SecureVaultPassVarLine,
            ProtectedFilePath = ProtectedFilePath,
            ProtectedFileEntropy = ProtectedFileEntropy
        };

        private Mock<IConfigurationReader> _configurationReaderMock;
        private Mock<IEnvironmentVariableReader> _environmentVariableReaderMock;
        private Mock<IStringValidator> _stringValidatorMock;
        private Mock<IObjectValidator> _objectValidatorMock;
        private IConfigurationRepository _sut;

        [SetUp]
        public void Init()
        {
            _configurationReaderMock = new Mock<IConfigurationReader>();
            _environmentVariableReaderMock = new Mock<IEnvironmentVariableReader>();
            _stringValidatorMock = new Mock<IStringValidator>();
            _objectValidatorMock = new Mock<IObjectValidator>();

            _sut = new Environmentalist.Services.Repositories.ConfigurationRepository.ConfigurationRepository(
                _configurationReaderMock.Object,
                _environmentVariableReaderMock.Object,
                _stringValidatorMock.Object,
                _objectValidatorMock.Object);
        }

        [Test]
        public async Task When_get_configuration_Then_returns_configuration()
        {
            _configurationReaderMock.Setup(m => m.Read(Path)).ReturnsAsync(Configuration);
            _environmentVariableReaderMock.Setup(m => m.Read(It.IsAny<IEnumerable<string>>())).Returns(EnvVarValues);

            var result = await _sut.GetConfiguration(Path);

            Assert.IsNotNull(result);
            Assert.AreEqual(TemplatePath, result.TemplatePath);
            Assert.AreEqual(ResultPath, result.ResultPath);
            Assert.AreEqual(ProfilePath, result.ProfilePath);
            Assert.AreEqual(SecureVaultPathVarValue, result.SecureVaultPath);
            Assert.AreEqual(SecureVaultPassVarValue, result.SecureVaultPass);
            Assert.AreEqual(ProtectedFilePath, result.ProtectedFilePath);
            Assert.AreEqual(ProtectedFileEntropy, result.ProtectedFileEntropy);
        }

        [Test]
        public void When_get_configuration_And_path_is_invalid_then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetConfiguration(null));
        }
    }
}
