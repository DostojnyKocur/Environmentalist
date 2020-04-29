using System.Threading.Tasks;
using Environmentalist.Models;

namespace Environmentalist.Services.Repositories.ConfigurationRepository
{
    public interface IConfigurationRepository
    {
        Task<ConfigurationModel> GetConfiguration(string path);
    }
}
