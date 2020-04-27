using System.Threading.Tasks;
using Environmentalist.Models;
using Environmentalist.Services.ConfigurationReader;
using Environmentalist.Services.EnvironmentVariableReader;
using Environmentalist.Validators.StringValidator;

namespace Environmentalist.Services.ConfigurationRepository
{
    public sealed class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly IEnvironmentVariableReader _environmentVariableReader;
        private readonly IStringValidator _stringValidator;

        public ConfigurationRepository(
            IConfigurationReader configurationReader,
            IEnvironmentVariableReader environmentVariableReader,
            IStringValidator stringValidator)
        {
            _configurationReader = configurationReader;
            _environmentVariableReader = environmentVariableReader;
            _stringValidator = stringValidator;
        }

        public async Task<ConfigurationModel> GetConfiguration(string path)
        {
            _stringValidator.IsNullOrWhitespace(path, nameof(path));

            var configuration = await _configurationReader.Read(path);
            var configurationEnvVariables = _configurationReader.ExtractEnvironmentVariables(configuration);
            var configEnvVariablesValues = _environmentVariableReader.Read(configurationEnvVariables);
            configuration = _configurationReader.ProcessEnvironmentVariables(configuration, configEnvVariablesValues);

            return configuration;
        }
    }
}
