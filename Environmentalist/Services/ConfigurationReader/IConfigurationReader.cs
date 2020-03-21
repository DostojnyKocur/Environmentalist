using System.Threading.Tasks;
using Environmentalist.Models;

namespace Environmentalist.Services.ConfigurationReader
{
    public interface IConfigurationReader
    {
        Task<ConfigurationModel> Read(string path);
    }
}
