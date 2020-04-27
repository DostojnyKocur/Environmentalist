using System.Threading.Tasks;
using Environmentalist.Models;

namespace Environmentalist.Services.ConfigurationRepository
{
    public interface IConfigurationRepository
    {
        Task<ConfigurationModel> GetConfiguration(string path);
    }
}
