using System.Threading.Tasks;
using Environmentalist.Models;

namespace Environmentalist.Services.Repositories.Pbkdf2Repository
{
    public interface IPbkdf2Repository
    {
        Task Encrypt(string sourcePath, string outputPath);
        Task<Pbkdf2Model> Decrypt(string path, string entropy);
    }
}
