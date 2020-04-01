using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Environmentalist.Services.DiskService;
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
        private const string ValidNewFilePath = @"test/path";
        private const string FileContent = "SomeFile SomeContent 42";
        private const string FileNewContent = "SomeNewFile SomeNewContent 42";

        private IFileSystem _fileSystemMock;
        private Mock<IFileValidator> _fileValidatorMock;
        private Mock<IStringValidator> _stringValidatorMock;
        private IDiskService _sut;

        [SetUp]
        public void Init()
        {
            _fileValidatorMock = new Mock<IFileValidator>();
            _stringValidatorMock = new Mock<IStringValidator>();
            _fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { ValidFilePath, new MockFileData(FileContent) },
            });

            _sut = new Environmentalist.Services.DiskService.DiskService(
                _fileSystemMock,
                _fileValidatorMock.Object,
                _stringValidatorMock.Object);
        }

        [Test]
        public async Task When_read_file_text_Then_returns_contetn()
        {
            var contetnt = await _sut.ReadFileText(ValidFilePath);

            Assert.AreEqual(FileContent, contetnt);
        }

        [Test]
        public void When_read_file_text_And_path_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(It.IsAny<string>(), It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ReadFileText(null));
        }

        [Test]
        public void When_read_file_text_And_file_does_not_exist_Then_throws_file_not_found_exception()
        {
            _fileValidatorMock.Setup(m => m.IsExist(It.IsAny<string>())).Throws(new FileNotFoundException());

            Assert.ThrowsAsync<FileNotFoundException>(() => _sut.ReadFileText(null));
        }

        [Test]
        public async Task When_write_file_text_Then_returns_contetn()
        {
            await _sut.WriteFileText(FileNewContent, ValidFilePath);

            var contetnt = _fileSystemMock.File.ReadAllText(ValidFilePath);

            Assert.AreEqual(FileNewContent, contetnt);
        }

        [Test]
        public void When_write_file_text_And_path_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.WriteFileText(FileNewContent, null));
        }

        [Test]
        public void When_write_file_text_And_content_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.WriteFileText(null, ValidFilePath));
        }
    }
}
