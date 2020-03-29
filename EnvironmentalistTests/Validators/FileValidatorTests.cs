using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Environmentalist.Validators.FileValidator;
using NUnit.Framework;

namespace EnvironmentalistTests.Validators
{
    [TestFixture]
    public class FileValidatorTests
    {
        private const string ValidFilePath = @"test/path";
        private const string InvalidFilePath = @"invalid/test/path";

        private IFileSystem _fileSystemMock;
        private IFileValidator _sut;

        [SetUp]
        public void Init()
        {
            _fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { ValidFilePath, new MockFileData(string.Empty) },
            });
            _sut = new FileValidator(_fileSystemMock);
        }

        [Test]
        public void When_is_exist_And_file_exists_Then_does_not_throw_any_exception()
        {

            Assert.DoesNotThrow(() => _sut.IsExist(ValidFilePath));
        }

        [Test]
        public void When_is_exist_And_file_is_not_exist_Then_throws_file_not_found_exception()
        {
            Assert.Throws<FileNotFoundException>(() => _sut.IsExist(InvalidFilePath));
        }
    }
}
