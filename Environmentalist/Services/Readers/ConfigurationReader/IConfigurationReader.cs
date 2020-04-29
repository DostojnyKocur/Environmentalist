using System.Threading.Tasks;
using Environmentalist.Models;

namespace Environmentalist.Services.Readers.ConfigurationReader
{
    public interface IConfigurationReader
    {
        Task<ConfigurationModel> Read(string path);
    }
}
