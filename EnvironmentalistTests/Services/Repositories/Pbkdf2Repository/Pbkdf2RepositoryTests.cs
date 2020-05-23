using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Environmentalist;
using Environmentalist.Services.DiskService;
using Environmentalist.Services.Pbkdf2Service;
using Environmentalist.Services.Readers.EnvironmentVariableReader;
using Environmentalist.Services.Repositories.Pbkdf2Repository;
using Environmentalist.Validators.FileValidator;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;
using Moq;
using NUnit.Framework;

namespace EnvironmentalistTests.Services.Repositories.PbkdF2Repository
{
    [TestFixture]
    public class Pbkdf2RepositoryTests
    {
        private const string OutputDirectoryName = "directory";
        private const string OutputFileExtension = ".txt";
        private const string OutputFileName = "result_file";
        private const string SourcePath = "source_path";
        private const string ProtectedFilePath = "protected_file_path";
        private const string SourceFileContent = nameof(SourceFileContent);
        private const string ProtectedFileContent = nameof(ProtectedFileContent);
        private const string ProtectedFileEntropy = nameof(ProtectedFileEntropy);

        private static readonly string OutputPath = $@"{OutputDirectoryName}\{OutputFileName}{OutputFileExtension}";
        private static readonly string OutputEntropyPath = $@"{OutputDirectoryName}\{OutputFileName}{Consts.EntropyFileSuffix}{OutputFileExtension}";

        private const string Key1 = nameof(Key1);
        private const string Key2 = nameof(Key2);
        private const string Value1 = nameof(Value1);
        private const string Value2 = nameof(Value2);

        private static readonly string ProtectedFileDecryptedContent = @$"
                                    {Key1}={Value1}
                                    {Key2}={Value2}";

        private Mock<IPbkdf2Service> _pbkdf2ServiceMock;
        private Mock<IEnvironmentVariableReader> _environmentVariableReaderMock;
        private MockFileSystem _fileSystemMock;
        private Mock<IDiskService> _diskServiceMock;
        private Mock<IFileValidator> _fileValidatorMock;
        private Mock<IStringValidator> _stringValidatorMock;
        private Mock<IObjectValidator> _objectValidatorMock;
        private IPbkdf2Repository _sut;

        [SetUp]
        public void Init()
        {
            _pbkdf2ServiceMock = new Mock<IPbkdf2Service>();
            _environmentVariableReaderMock = new Mock<IEnvironmentVariableReader>();
            _fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { SourcePath, new MockFileData(SourceFileContent) },
            });
            _diskServiceMock = new Mock<IDiskService>();
            _fileValidatorMock = new Mock<IFileValidator>();
            _stringValidatorMock = new Mock<IStringValidator>();
            _objectValidatorMock = new Mock<IObjectValidator>();

            _sut = new Environmentalist.Services.Repositories.Pbkdf2Repository.Pbkdf2Repository(
                _pbkdf2ServiceMock.Object,
                _environmentVariableReaderMock.Object,
                _fileSystemMock,
                _diskServiceMock.Object,
                _fileValidatorMock.Object,
                _stringValidatorMock.Object,
                _objectValidatorMock.Object);
        }

        [Test]
        public async Task When_encrypt_Then_write_to_files()
        {
            _diskServiceMock.Setup(m => m.ReadFileText(SourcePath)).ReturnsAsync(SourceFileContent);
            _pbkdf2ServiceMock.Setup(m => m.Encrypt(SourceFileContent)).Returns((cipherText: ProtectedFileContent, entropy: ProtectedFileEntropy));

            await _sut.Encrypt(SourcePath, OutputPath);

            _diskServiceMock.Verify(m => m.WriteFileText(ProtectedFileContent, OutputPath), Times.Once);
            _diskServiceMock.Verify(m => m.WriteFileText(ProtectedFileEntropy, OutputEntropyPath), Times.Once);
        }

        [Test]
        public void When_encrypt_And_source_path_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Encrypt(null, OutputPath));
        }

        [Test]
        public void When_encrypt_And_output_path_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Encrypt(null, OutputPath));
        }

        [Test]
        public void When_encrypt_And_source_file_does_not_exist_Then_throws_file_not_found_exception() 
        { 
            _fileValidatorMock.Setup(m => m.IsExist(SourcePath)).Throws(new FileNotFoundException());

            Assert.ThrowsAsync<FileNotFoundException>(() => _sut.Encrypt(SourcePath, OutputPath));
        }

        [Test]
        public void When_encrypt_And_output_file_already_exist_Then_throws_argument_exception()
        {
            _fileValidatorMock.Setup(m => m.IsNotExist(OutputPath)).Throws(new ArgumentException());

            Assert.ThrowsAsync<ArgumentException>(() => _sut.Encrypt(SourcePath, OutputPath));
        }

        [Test]
        public void When_encrypt_And_output_entrupy_file_already_exist_Then_throws_argument_exception()
        {
            _fileValidatorMock.Setup(m => m.IsNotExist(OutputEntropyPath)).Throws(new ArgumentException());

            Assert.ThrowsAsync<ArgumentException>(() => _sut.Encrypt(SourcePath, OutputPath));
        }

        [Test]
        public async Task When_decrypt_Then_returns_pbkdf2_model()
        {
            _diskServiceMock.Setup(m => m.ReadFileText(ProtectedFilePath)).ReturnsAsync(ProtectedFileContent);
            _pbkdf2ServiceMock.Setup(m => m.Decrypt(ProtectedFileContent, ProtectedFileEntropy)).Returns(ProtectedFileDecryptedContent);
            _environmentVariableReaderMock.Setup(m => m.Read(It.IsAny<IEnumerable<string>>())).Returns(new Dictionary<string, string>());

            var model = await _sut.Decrypt(ProtectedFilePath, ProtectedFileEntropy);

            Assert.IsNotNull(model);
            Assert.AreEqual(Value1, model.Fields[Key1]);
            Assert.AreEqual(Value2, model.Fields[Key2]);
        }

        [Test]
        public void When_decrypt_And_path_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Decrypt(null, ProtectedFileEntropy));
        }

        [Test]
        public void When_decrypt_And_entrupy_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Decrypt(ProtectedFilePath, null));
        }

        [Test]
        public void When_decrypt_And_source_file_does_not_exist_Then_throws_file_not_found_exception()
        {
            _fileValidatorMock.Setup(m => m.IsExist(ProtectedFilePath)).Throws(new FileNotFoundException());

            Assert.ThrowsAsync<FileNotFoundException>(() => _sut.Decrypt(ProtectedFilePath, ProtectedFileEntropy));
        }
    }
}
