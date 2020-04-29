using System;
using System.IO;
using System.Threading.Tasks;
using Environmentalist.Services.DiskService;
using Environmentalist.Services.Readers.ConfigurationReader;
using Environmentalist.Validators.FileValidator;
using Environmentalist.Validators.StringValidator;
using Moq;
using NUnit.Framework;

namespace EnvironmentalistTests.Services.Readers.ConfigurationReader
{
    [TestFixture]
    public class ConfigurationReaderTests
    {
        private const string Path = "testpath";
        private const string TemplatePath = "template.env";
        private const string ResultPath = "result.env";
        private const string ProfilePath = "profile.txt";
        private const string SecureVaultPath = "keepass.kdbx";
        private const string SecureVaultPass = "pass";

        private static readonly string BasicValidConfigFile = @$"
                                    templatePath={TemplatePath}
                                    resultPath={ResultPath}
                                    profilePath={ProfilePath}
                                    secureVaultPath={SecureVaultPath}
                                    secureVaultPass={SecureVaultPass}";

        private Mock<IDiskService> _diskServiceMock;
        private Mock<IFileValidator> _fileValidatorMock;
        private Mock<IStringValidator> _stringValidatorMock;
        private IConfigurationReader _sut;

        [SetUp]
        public void Init()
        {
            _diskServiceMock = new Mock<IDiskService>();
            _fileValidatorMock = new Mock<IFileValidator>();
            _stringValidatorMock = new Mock<IStringValidator>();

            _sut = new Environmentalist.Services.Readers.ConfigurationReader.ConfigurationReader(
                _diskServiceMock.Object,
                _fileValidatorMock.Object,
                _stringValidatorMock.Object);
        }

        [Test]
        public async Task When_read_Then_returns_configuration()
        {
            _diskServiceMock.Setup(m => m.ReadFileText(Path)).ReturnsAsync(BasicValidConfigFile);

            var configuration = await _sut.Read(Path);

            Assert.AreEqual(TemplatePath, configuration.TemplatePath);
            Assert.AreEqual(ResultPath, configuration.ResultPath);
            Assert.AreEqual(ProfilePath, configuration.ProfilePath);
            Assert.AreEqual(SecureVaultPath, configuration.SecureVaultPath);
            Assert.AreEqual(SecureVaultPass, configuration.SecureVaultPass);
        }

        [Test]
        public void When_read_And_path_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Read(null));
        }

        [Test]
        public void When_read_And_file_does_not_exist_Then_throws_file_not_found_exception()
        {
            _fileValidatorMock.Setup(m => m.IsExist(It.IsAny<string>())).Throws(new FileNotFoundException());

            Assert.ThrowsAsync<FileNotFoundException>(() => _sut.Read(null));
        }
    }
}
