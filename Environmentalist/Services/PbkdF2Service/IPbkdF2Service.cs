namespace Environmentalist.Services.Pbkdf2Service
{
    public interface IPbkdf2Service
    {
        (string cipherText, string entropy) Encrypt(string content);
        string Decrypt(string content, string entropy);
    }
}
