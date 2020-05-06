using System.Security.Cryptography;
using System.Text;
using Environmentalist.Validators.StringValidator;

namespace Environmentalist.Services.PbkdF2Service
{
    public sealed class PbkdF2Service : IPbkdF2Service
    {
        private readonly IStringValidator _stringValidator;

        public PbkdF2Service(IStringValidator stringValidator)
        {
            _stringValidator = stringValidator;
        }

        public (string cipherText, string entropy) Encrypt(string content)
        {
            _stringValidator.IsNullOrWhitespace(content, nameof(content));

            var contentBytes = Encoding.UTF8.GetBytes(content);
            var entropy = GetEntropy();

            var ciphertext = ProtectedData.Protect(contentBytes, entropy, DataProtectionScope.CurrentUser);

            var cipherTextString = System.Convert.ToBase64String(ciphertext);
            var entropyString = System.Convert.ToBase64String(entropy);

            return (cipherTextString, entropyString);
        }

        public string Decrypt(string content, string entropy)
        {
            _stringValidator.IsNullOrWhitespace(content, nameof(content));
            _stringValidator.IsNullOrWhitespace(entropy, nameof(entropy));

            var contentBytes = System.Convert.FromBase64String(content);
            var entropyBytes = System.Convert.FromBase64String(entropy);

            var plainTextBytes = ProtectedData.Unprotect(contentBytes, entropyBytes, DataProtectionScope.CurrentUser);

            var plainTextString = Encoding.UTF8.GetString(plainTextBytes);

            return plainTextString;
        }

        private static byte[] GetEntropy()
        {
            var entropy = new byte[Consts.EntropySize];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetBytes(entropy);
            }

            return entropy;
        }
    }
}
