using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Environmentalist.Models;
using Environmentalist.Services.DiskService;
using Environmentalist.Services.Readers.ProfileReader;
using Environmentalist.Validators.FileValidator;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;
using Moq;
using NUnit.Framework;

namespace EnvironmentalistTests.Services.Readers.ProfileReader
{
    [TestFixture]
    public class ProfileReaderTests
    {
        private const string Path = "testpath";
        private const string Key1 = nameof(Key1);
        private const string Key2 = nameof(Key2);
        private const string KeyEnv1 = nameof(KeyEnv1);
        private const string KeyEnv2 = nameof(KeyEnv2);
        private const string Value1 = nameof(Value1);
        private const string Value2 = nameof(Value2);
        private const string ValueEnv1 = nameof(ValueEnv1);
        private const string ValueEnv2 = nameof(ValueEnv2);
        private const string Env1Value = nameof(Env1Value);
        private const string Env2Value = nameof(Env2Value);

        private static readonly string ValueEnv1Line = $"[EnvVar]({ValueEnv1})";
        private static readonly string ValueEnv2Line = $"[EnvVar]({ValueEnv2})";

        private static readonly string BasicValidTemplateFile = @$"
                                    {Key1}={Value1}
                                    {Key2}={Value2}
                                    {KeyEnv1}={ValueEnv1Line}
                                    {KeyEnv2}={ValueEnv2Line}";

        private ProfileModel EnvVarConfigurationModel;

        private Mock<IDiskService> _diskServiceMock;
        private Mock<IFileValidator> _fileValidatorMock;
        private Mock<IStringValidator> _stringValidatorMock;
        private Mock<IObjectValidator> _objectValidatorMock;
        private IProfileReader _sut;

        [SetUp]
        public void Init()
        {
            EnvVarConfigurationModel = new ProfileModel();
            EnvVarConfigurationModel.Fields.Add(Key1, Value1);
            EnvVarConfigurationModel.Fields.Add(Key2, Value2);
            EnvVarConfigurationModel.Fields.Add(KeyEnv1, ValueEnv1Line);
            EnvVarConfigurationModel.Fields.Add(KeyEnv2, ValueEnv2Line);

            _diskServiceMock = new Mock<IDiskService>();
            _fileValidatorMock = new Mock<IFileValidator>();
            _stringValidatorMock = new Mock<IStringValidator>();
            _objectValidatorMock = new Mock<IObjectValidator>();

            _sut = new Environmentalist.Services.Readers.ProfileReader.ProfileReader(
                _diskServiceMock.Object,
                _fileValidatorMock.Object,
                _stringValidatorMock.Object,
                _objectValidatorMock.Object);
        }

        [Test]
        public async Task When_read_Then_returns_template()
        {
            _diskServiceMock.Setup(m => m.ReadFileText(Path)).ReturnsAsync(BasicValidTemplateFile);

            var model = await _sut.Read(Path);

            Assert.AreEqual(4, model.Fields.Count);
            Assert.AreEqual(Value1, model.Fields[Key1]);
            Assert.AreEqual(Value2, model.Fields[Key2]);
            Assert.AreEqual(ValueEnv1Line, model.Fields[KeyEnv1]);
            Assert.AreEqual(ValueEnv2Line, model.Fields[KeyEnv2]);
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

        [Test]
        public void When_extract_environment_variables_Then_returns_list_of_environment_variables()
        {
            var envVarList = _sut.ExtractEnvironmentVariables(EnvVarConfigurationModel).ToList();

            Assert.AreEqual(2, envVarList.Count);
            Assert.AreEqual(ValueEnv1, envVarList[0]);
            Assert.AreEqual(ValueEnv2, envVarList[1]);
        }

        [Test]
        public void When_extract_environment_variables_And_parameter_is_null_Then_throws_argument_null_exception()
        {
            _objectValidatorMock.Setup(m => m.IsNull(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _sut.ExtractEnvironmentVariables(null));
        }
    }
}
