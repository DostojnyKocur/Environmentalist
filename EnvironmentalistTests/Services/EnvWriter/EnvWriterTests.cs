using Environmentalist.Models;
using Environmentalist.Services.DiskService;
using Environmentalist.Services.EnvWriter;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace EnvironmentalistTests.Services.EnvWriter
{
    [TestFixture]
    public class EnvWriterTests
    {
        private const string Field1 = nameof(Field1);
        private const string Field2 = nameof(Field2);
        private const string Value1 = nameof(Value1);
        private const string Value2 = nameof(Value2);
        private const string ResultPath = "test path";
        private const string TemplatePath = "test path";

        private Mock<IDiskService> _dickServiceModk;
        private Mock<IStringValidator> _stringValidatorMock;
        private Mock<IObjectValidator> _objectValidatorMock;
        private IEnvWriter _sut;

        [SetUp]
        public void Init()
        {
            _dickServiceModk = new Mock<IDiskService>();
            _stringValidatorMock = new Mock<IStringValidator>();
            _objectValidatorMock = new Mock<IObjectValidator>();

            _sut = new Environmentalist.Services.EnvWriter.EnvWriter(
                _dickServiceModk.Object,
                _stringValidatorMock.Object,
                _objectValidatorMock.Object);
        }

        [Test]
        public async Task When_write_Then_write_to_file()
        {
            var model = PrepareTemplateModel();
            _dickServiceModk.Setup(m => m.ReadFileText(TemplatePath)).ReturnsAsync(PrepareTemplateFileContent());

            await _sut.Write(model, ResultPath, TemplatePath);

            _dickServiceModk.Verify(m => m.WriteFileText(PrepareFileContent(), ResultPath), Times.Once);
        }

        [Test]
        public void When_write_And_output_path_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());
            var model = PrepareTemplateModel();

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Write(model, null, TemplatePath));
        }

        [Test]
        public void When_write_And_template_path_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());
            var model = PrepareTemplateModel();

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Write(model, ResultPath, null));
        }

        [Test]
        public void When_write_And_model_is_null_Then_throws_argument_null_exception()
        {
            _objectValidatorMock.Setup(m => m.IsNull(null, It.IsAny<string>())).Throws(new ArgumentNullException());
            var model = PrepareTemplateModel();

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Write(null, ResultPath, TemplatePath));
        }

        private static TemplateModel PrepareTemplateModel()
        {
            var result = new TemplateModel();
            result.Fields.Add(Field1, Value1);
            result.Fields.Add(Field2, Value2);

            return result;
        }

        private static string PrepareFileContent()
        {
            return $@"
                    {Field1}=     {Value1}{Environment.NewLine}
                    {Field2}   = {Value2}{Environment.NewLine}";
        }

        private static string PrepareTemplateFileContent()
        {
            return $@"
                    {Field1}=     placeholder1   {Environment.NewLine}
                    {Field2}   = placeholder2 {Environment.NewLine}";
        }
    }
}
