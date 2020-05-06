using System.Threading.Tasks;

namespace Environmentalist.Services.PbkdF2Service
{
    public interface IPbkdF2Service
    {
        (string cipherText, string entropy) Encrypt(string content);
        string Decrypt(string content, string entropy);
    }
}
