using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Environmentalist.Models;
using Environmentalist.Services.ConfigurationReader;
using Environmentalist.Services.ConfigurationRepository;
using Environmentalist.Services.EnvironmentVariableReader;
using Environmentalist.Validators.StringValidator;
using Moq;
using NUnit.Framework;

namespace EnvironmentalistTests.Services.ConfigurationRepository
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

        private const string SecureVaultPathVarValue = nameof(SecureVaultPathVarValue);
        private const string SecureVaultPassVarValue = nameof(SecureVaultPassVarValue);

        private static readonly string SecureVaultPathVarLine = $"[EnvVar]({SecureVaultPathVarName})";
        private static readonly string SecureVaultPassVarLine = $"[EnvVar]({SecureVaultPassVarName})";

        private static readonly List<string> EnvVarList = new List<string>
        {
            SecureVaultPathVarName, SecureVaultPassVarName
        };

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
            SecureVaultPass = SecureVaultPassVarLine
        };

        private readonly ConfigurationModel FinalConfiguration = new ConfigurationModel
        {
            TemplatePath = TemplatePath,
            ResultPath = ResultPath,
            ProfilePath = ProfilePath,
            SecureVaultPath = SecureVaultPathVarValue,
            SecureVaultPass = SecureVaultPassVarValue
        };

        private Mock<IConfigurationReader> _configurationReaderMock;
        private Mock<IEnvironmentVariableReader> _environmentVariableReaderMock;
        private Mock<IStringValidator> _stringValidatorMock;
        private IConfigurationRepository _sut;

        [SetUp]
        public void Init()
        {
            _configurationReaderMock = new Mock<IConfigurationReader>();
            _environmentVariableReaderMock = new Mock<IEnvironmentVariableReader>();
            _stringValidatorMock = new Mock<IStringValidator>();

            _sut = new Environmentalist.Services.ConfigurationRepository.ConfigurationRepository(
                _configurationReaderMock.Object,
                _environmentVariableReaderMock.Object,
                _stringValidatorMock.Object);
        }

        [Test]
        public async Task When_get_configuration_Then_returns_configuration()
        {
            _configurationReaderMock.Setup(m => m.Read(Path)).ReturnsAsync(Configuration);
            _configurationReaderMock.Setup(m => m.ExtractEnvironmentVariables(Configuration)).Returns(EnvVarList);
            _configurationReaderMock.Setup(m => m.ProcessEnvironmentVariables(Configuration, EnvVarValues)).Returns(FinalConfiguration);
            _environmentVariableReaderMock.Setup(m => m.Read(It.IsAny<IEnumerable<string>>())).Returns(EnvVarValues);

            var result = await _sut.GetConfiguration(Path);

            Assert.IsNotNull(result);
            Assert.AreEqual(TemplatePath, result.TemplatePath);
            Assert.AreEqual(ResultPath, result.ResultPath);
            Assert.AreEqual(ProfilePath, result.ProfilePath);
            Assert.AreEqual(SecureVaultPathVarValue, result.SecureVaultPath);
            Assert.AreEqual(SecureVaultPassVarValue, result.SecureVaultPass);
        }

        [Test]
        public void When_get_configuration_And_path_is_invalid_then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetConfiguration(null));
        }
    }
}
