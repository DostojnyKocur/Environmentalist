using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Environmentalist.Models;
using Environmentalist.Services.ConfigurationReader;
using Environmentalist.Services.DiskService;
using Environmentalist.Validators.FileValidator;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;
using Moq;
using NUnit.Framework;

namespace EnvironmentalistTests.Services.ConfigurationReader
{
    [TestFixture]
    public class ConfigurationReaderTests
    {
        private const string Path = "testpath";
        private const string TemplatePath = "template.env";
        private const string ResultPath = "result.env";
        private const string ConfigPath = "conf.txt";
        private const string SecureVaultPath = "keepass.kdbx";
        private const string SecureVaultPass = "pass";

        private const string TemplatePathVarName = "template";
        private const string ResultPathVarName = "result";
        private const string ConfigPathVarName = "conf";
        private const string SecureVaultPathVarName = "keepass";
        private const string SecureVaultPassVarName = "pass";

        private const string TemplatePathVarValue = nameof(TemplatePathVarValue);
        private const string ResultPathVarValue = nameof(ResultPathVarValue);
        private const string ConfigPathVarValue = nameof(ConfigPathVarValue);
        private const string SecureVaultPathVarValue = nameof(SecureVaultPathVarValue);
        private const string SecureVaultPassVarValue = nameof(SecureVaultPassVarValue);

        private static readonly string TemplatePathVarLine = $"[EnvVar]({TemplatePathVarName})";
        private static readonly string ResultPathVarLine = $"[EnvVar]({ResultPathVarName})";
        private static readonly string ConfigPathVarLine = $"[EnvVar]({ConfigPathVarName})";
        private static readonly string SecureVaultPathVarLine = $"[EnvVar]({SecureVaultPathVarName})";
        private static readonly string SecureVaultPassVarLine = $"[EnvVar]({SecureVaultPassVarName})";

        private static readonly string BasicValidConfigFile = @$"
                                    templatePath={TemplatePath}
                                    resultPath={ResultPath}
                                    configPath={ConfigPath}
                                    secureVaultPath={SecureVaultPath}
                                    secureVaultPass={SecureVaultPass}";

        private static readonly Dictionary<string, string> EnvVarValues = new Dictionary<string, string>
        {
            { TemplatePathVarName, TemplatePathVarValue },
            { ResultPathVarName, ResultPathVarValue },
            { ConfigPathVarName, ConfigPathVarValue },
            { SecureVaultPathVarName, SecureVaultPathVarValue },
            { SecureVaultPassVarName, SecureVaultPassVarValue },
        };

        private static readonly ConfigurationModel EnvVarConfigurationModel = new ConfigurationModel
        {
            TemplatePath = TemplatePathVarLine,
            ResultPath = ResultPathVarLine,
            ConfigPath = ConfigPathVarLine,
            SecureVaultPath = SecureVaultPathVarLine,
            SecureVaultPass = SecureVaultPassVarLine
        };

        private Mock<IDiskService> _diskServiceMock;
        private Mock<IFileValidator> _fileValidatorMock;
        private Mock<IStringValidator> _stringValidatorMock;
        private Mock<IObjectValidator> _objectValidatorMock;
        private IConfigurationReader _sut;

        [SetUp]
        public void Init()
        {
            _diskServiceMock = new Mock<IDiskService>();
            _fileValidatorMock = new Mock<IFileValidator>();
            _stringValidatorMock = new Mock<IStringValidator>();
            _objectValidatorMock = new Mock<IObjectValidator>();

            _sut = new Environmentalist.Services.ConfigurationReader.ConfigurationReader(
                _diskServiceMock.Object,
                _fileValidatorMock.Object,
                _stringValidatorMock.Object,
                _objectValidatorMock.Object);
        }

        [Test]
        public async Task When_read_Then_returns_configuration()
        {
            _diskServiceMock.Setup(m => m.ReadFileText(Path)).ReturnsAsync(BasicValidConfigFile);

            var configuration = await _sut.Read(Path);

            Assert.AreEqual(TemplatePath, configuration.TemplatePath);
            Assert.AreEqual(ResultPath, configuration.ResultPath);
            Assert.AreEqual(ConfigPath, configuration.ConfigPath);
            Assert.AreEqual(SecureVaultPath, configuration.SecureVaultPath);
            Assert.AreEqual(SecureVaultPass, configuration.SecureVaultPass);
        }

        [Test]
        public void When_read_And_path_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(It.IsAny<string>(), It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Read(null));
        }

        [Test]
        public void When_read_And_file_does_not_exist_Then_throws_file_not_found_exception()
        {
            _fileValidatorMock.Setup(m => m.IsExist(It.IsAny<string>())).Throws(new FileNotFoundException());

            Assert.ThrowsAsync<FileNotFoundException>(() => _sut.Read(null));
        }

        [Test]
        public void When_extract_environment_variables_Then_returns_list_of_environment_variables()
        {
            var envVarList = _sut.ExtractEnvironmentVariables(EnvVarConfigurationModel).ToList();

            Assert.AreEqual(5, envVarList.Count);
            Assert.AreEqual(TemplatePathVarName, envVarList[0]);
            Assert.AreEqual(ResultPathVarName, envVarList[1]);
            Assert.AreEqual(ConfigPathVarName, envVarList[2]);
            Assert.AreEqual(SecureVaultPathVarName, envVarList[3]);
            Assert.AreEqual(SecureVaultPassVarName, envVarList[4]);
        }

        [Test]
        public void When_extract_environment_variables_And_parameter_is_null_Then_throws_argument_null_exception()
        {
            _objectValidatorMock.Setup(m => m.IsNull(It.IsAny<object>(), It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _sut.ExtractEnvironmentVariables(null));
        }

        [Test]
        public void When_process_environment_variables_Then_returns_new_configuration()
        {
            var processedConfig = _sut.ProcessEnvironmentVariables(EnvVarConfigurationModel, EnvVarValues);

            Assert.AreEqual(TemplatePathVarValue, processedConfig.TemplatePath);
            Assert.AreEqual(ResultPathVarValue, processedConfig.ResultPath);
            Assert.AreEqual(ConfigPathVarValue, processedConfig.ConfigPath);
            Assert.AreEqual(SecureVaultPathVarValue, processedConfig.SecureVaultPath);
            Assert.AreEqual(SecureVaultPassVarValue, processedConfig.SecureVaultPass);
        }

        [Test]
        public void When_process_environment_variables_And_configuration_is_null_Then_throws_argument_null_exception()
        {
            _objectValidatorMock.Setup(m => m.IsNull(It.IsAny<object>(), It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _sut.ProcessEnvironmentVariables(null, EnvVarValues));
        }

        [Test]
        public void When_process_environment_variables_And_environment_variables_dictionary_is_null_Then_throws_argument_null_exception()
        {
            _objectValidatorMock.Setup(m => m.IsNull(It.IsAny<object>(), It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _sut.ProcessEnvironmentVariables(EnvVarConfigurationModel, null));
        }
    }
}
