using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Environmentalist.Models;
using Environmentalist.Services.EnvironmentVariableReader;
using Environmentalist.Services.ProfileReader;
using Environmentalist.Services.ProfileRepository;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;
using Moq;
using NUnit.Framework;

namespace EnvironmentalistTests.Services.ProfileRepository
{
    [TestFixture]
    public class ProfileRepositoryTests
    {
        private const string Path = @"some path";
        private const string Field1 = nameof(Field1);
        private const string Field2 = nameof(Field2);
        private const string Value1 = nameof(Value1);
        private const string SecureVaultPathVarName = "keepass";

        private const string SecureVaultPathVarValue = nameof(SecureVaultPathVarValue);
        private const string SecureVaultPassVarValue = nameof(SecureVaultPassVarValue);

        private static readonly string SecureVaultPathVarLine = $"[EnvVar]({SecureVaultPathVarName})";

        private static readonly Dictionary<string, string> EnvVarValues = new Dictionary<string, string>
        {
            { SecureVaultPathVarName, SecureVaultPathVarValue }
        };

        private static ProfileModel Profile;

        private Mock<IProfileReader> _profileReaderMock;
        private Mock<IEnvironmentVariableReader> _environmentVariableReaderMock;
        private Mock<IStringValidator> _stringValidatorMock;
        private Mock<IObjectValidator> _objectValidatorMock;
        private IProfileRepository _sut;

        [SetUp]
        public void Init()
        {
            Profile = new ProfileModel();
            Profile.Fields.Add(Field1, Value1);
            Profile.Fields.Add(Field2, SecureVaultPathVarLine);

            _profileReaderMock = new Mock<IProfileReader>();
            _environmentVariableReaderMock = new Mock<IEnvironmentVariableReader>();
            _stringValidatorMock = new Mock<IStringValidator>();
            _objectValidatorMock = new Mock<IObjectValidator>();

            _sut = new Environmentalist.Services.ProfileRepository.ProfileRepository(
                _profileReaderMock.Object,
                _environmentVariableReaderMock.Object,
                _stringValidatorMock.Object,
                _objectValidatorMock.Object);
        }

        [Test]
        public async Task When_get_profile_Then_returns_template()
        {
            _profileReaderMock.Setup(m => m.Read(Path)).ReturnsAsync(Profile);
            _environmentVariableReaderMock.Setup(m => m.Read(It.IsAny<IEnumerable<string>>())).Returns(EnvVarValues);

            var result = await _sut.GetProfile(Path);

            Assert.IsNotNull(result);
            Assert.AreEqual(Value1, result.Fields[Field1]);
            Assert.AreEqual(SecureVaultPathVarValue, result.Fields[Field2]);
        }

        [Test]
        public void When_get_profile_And_path_is_invalid_then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(null, It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetProfile(null));
        }
    }
}
