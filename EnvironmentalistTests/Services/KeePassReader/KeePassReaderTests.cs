using Environmentalist.Services.KeePassReader;
using Environmentalist.Validators.FileValidator;
using Environmentalist.Validators.StringValidator;
using Moq;
using NUnit.Framework;
using System;
using System.IO;

namespace EnvironmentalistTests.Services.KeePassReader
{
    [TestFixture]
    public class KeePassReaderTests
    {
        private const string Path = @"some path";
        private const string Password = @"some pass";

        private Mock<IFileValidator> _fileValidatorMock;
        private Mock<IStringValidator> _stringValidatorMock;
        private IKeePassReader _sut;

        [SetUp]
        public void Init()
        {
            _fileValidatorMock = new Mock<IFileValidator>();
            _stringValidatorMock = new Mock<IStringValidator>();
            _sut = new Environmentalist.Services.KeePassReader.KeePassReader(
                _fileValidatorMock.Object,
                _stringValidatorMock.Object);
        }

        [Test]
        public void When_read_database_And_path_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _sut.ReadDatabase(null, Password));
        }

        [Test]
        public void When_read_database_And_password_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _sut.ReadDatabase(Path, null));
        }

        [Test]
        public void When_read_database_And_database_does_not_exist_Then_throws_file_not_found_exception()
        {
            _fileValidatorMock.Setup(m => m.IsExist(It.IsAny<string>())).Throws(new FileNotFoundException());

            Assert.Throws<FileNotFoundException>(() => _sut.ReadDatabase(Path, Password));
        }
    }
}
