using System;
using Environmentalist.Services.PbkdF2Service;
using Environmentalist.Validators.StringValidator;
using Moq;
using NUnit.Framework;

namespace EnvironmentalistTests.Services.PbkdF2Service
{
    [TestFixture]
    public class PbkdF2ServiceTests
    {
        private const string Plaintext = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis ante ante, lobortis quis mollis ut, fringilla volutpat nisi. Proin facilisis posuere sollicitudin. Nullam imperdiet ut lacus vel commodo. Nulla facilisi. Proin in eleifend orci, id vehicula leo. Donec quam eros, euismod accumsan nisi sed, commodo varius massa. Vestibulum vel feugiat ante.";
        private const string Ciphertext = "cipher text";
        private const string Entropy = "entropy";

        private Mock<IStringValidator> _stringValidatorMock;
        private IPbkdF2Service _sut;

        [SetUp]
        public void Init()
        {
            _stringValidatorMock = new Mock<IStringValidator>();

            _sut = new Environmentalist.Services.PbkdF2Service.PbkdF2Service(
                _stringValidatorMock.Object);
        }

        [Test]
        public void When_encrypt_And_decrypt_Then_plaintext_are_the_same()
        {
            var cipher = _sut.Encrypt(Plaintext);
            var plaintext = _sut.Decrypt(cipher.cipherText, cipher.entropy);

            Assert.AreEqual(Plaintext, plaintext);
        }

        [Test]
        public void When_encrypt_And_context_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(It.IsAny<string>(), It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _sut.Encrypt(null));
        }

        [Test]
        public void When_decrypt_And_context_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(It.IsAny<string>(), It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _sut.Decrypt(null, Entropy));
        }

        [Test]
        public void When_decrypt_And_entropy_is_invalid_Then_throws_argument_null_exception()
        {
            _stringValidatorMock.Setup(m => m.IsNullOrWhitespace(It.IsAny<string>(), It.IsAny<string>())).Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _sut.Decrypt(Ciphertext, null));
        }
    }
}
