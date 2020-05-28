using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Environmentalist.Services.DiskService;
using Environmentalist.Validators;
using Environmentalist.Validators.FileValidator;
using Environmentalist.Validators.StringValidator;
using Moq;
using NUnit.Framework;

namespace EnvironmentalistTests.Services.DiskService
{
    [TestFixture]
    public class DiskServiceTests
    {
        private const string ValidFilePath = @"test/path";
        private const string FileContent = "SomeFile SomeContent 42";
        private const string FileNewContent = "SomeNewFile SomeNewContent 42";

        private IFileSystem _fileSystemMock;
        private Mock<IValidators> _validatorsMock;
        private IDiskService _sut;

        [SetUp]
        public void Init()
        {
            _validatorsMock = new Mock<IValidators>();
            _fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { ValidFilePath, new MockFileData(FileContent) },
            });

            _sut = new Environmentalist.Services.DiskService.DiskService(
                _fileSystemMock,
                _validatorsMock.Object);
        }

        [Test]
        public async Task When_read_file_text_Then_returns_contetn()
        {
            _validatorsMock.Setup(m => m.FileValidator).Returns(new Mock<IFileValidator>().Object);
            _validatorsMock.Setup(m => m.StringValidator).Returns(new Mock<IStringValidator>().Object);

            var contetnt = await _sut.ReadFileText(ValidFilePath);

            Assert.AreEqual(FileContent, contetnt);
        }

        [Test]
        public void When_read_file_text_And_path_is_invalid_Then_throws_argument_null_exception()
        {
            _validatorsMock.Setup(m => m.StringValidator.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ReadFileText(null));
        }

        [Test]
        public void When_read_file_text_And_file_does_not_exist_Then_throws_file_not_found_exception()
        {
            _validatorsMock.Setup(m => m.StringValidator).Returns(new Mock<IStringValidator>().Object);
            _validatorsMock.Setup(m => m.FileValidator.IsExist(null)).Throws(new FileNotFoundException());

            Assert.ThrowsAsync<FileNotFoundException>(() => _sut.ReadFileText(null));
        }

        [Test]
        public async Task When_write_file_text_Then_returns_contetn()
        {
            _validatorsMock.Setup(m => m.FileValidator).Returns(new Mock<IFileValidator>().Object);
            _validatorsMock.Setup(m => m.StringValidator).Returns(new Mock<IStringValidator>().Object);

            await _sut.WriteFileText(FileNewContent, ValidFilePath);

            var contetnt = _fileSystemMock.File.ReadAllText(ValidFilePath);

            Assert.AreEqual(FileNewContent, contetnt);
        }

        [Test]
        public void When_write_file_text_And_path_is_invalid_Then_throws_argument_null_exception()
        {
            _validatorsMock.Setup(m => m.StringValidator.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.WriteFileText(FileNewContent, null));
        }

        [Test]
        public void When_write_file_text_And_content_is_invalid_Then_throws_argument_null_exception()
        {
            _validatorsMock.Setup(m => m.StringValidator.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.WriteFileText(null, ValidFilePath));
        }
    }
}
