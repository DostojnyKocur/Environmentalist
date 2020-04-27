using System.Collections.Generic;
using System.Threading.Tasks;
using Environmentalist.Extensions;
using Environmentalist.Helpers;
using Environmentalist.Models;
using Environmentalist.Services.ConfigurationReader;
using Environmentalist.Services.EnvironmentVariableReader;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;

namespace Environmentalist.Services.ConfigurationRepository
{
    public sealed class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly IEnvironmentVariableReader _environmentVariableReader;
        private readonly IStringValidator _stringValidator;
        private readonly IObjectValidator _objectValidator;

        public ConfigurationRepository(
            IConfigurationReader configurationReader,
            IEnvironmentVariableReader environmentVariableReader,
            IStringValidator stringValidator,
            IObjectValidator objectValidator)
        {
            _configurationReader = configurationReader;
            _environmentVariableReader = environmentVariableReader;
            _stringValidator = stringValidator;
            _objectValidator = objectValidator;
        }

        public async Task<ConfigurationModel> GetConfiguration(string path)
        {
            _stringValidator.IsNullOrWhitespace(path, nameof(path));

            var configuration = await _configurationReader.Read(path);

            var environmentVariables = ExtractEnvironmentVariables(configuration);

            var environmentVariablesValues = _environmentVariableReader.Read(environmentVariables);

            configuration = ProcessEnvironmentVariables(configuration, environmentVariablesValues);

            return configuration;
        }

        private ICollection<string> ExtractEnvironmentVariables(ConfigurationModel model)
        {
            _objectValidator.IsNull(model, nameof(model));

            var foundEnvironmentVariables = new List<string>();

            if (model.TemplatePath.StartsWith(Consts.EnvironmentalVariableTagName))
            {
                foundEnvironmentVariables.Add(model.TemplatePath.GetBetweenParentheses());
            }
            if (model.ResultPath.StartsWith(Consts.EnvironmentalVariableTagName))
            {
                foundEnvironmentVariables.Add(model.ResultPath.GetBetweenParentheses());
            }
            if (model.ProfilePath.StartsWith(Consts.EnvironmentalVariableTagName))
            {
                foundEnvironmentVariables.Add(model.ProfilePath.GetBetweenParentheses());
            }
            if (model.SecureVaultPath.StartsWith(Consts.EnvironmentalVariableTagName))
            {
                foundEnvironmentVariables.Add(model.SecureVaultPath.GetBetweenParentheses());
            }
            if (model.SecureVaultPass.StartsWith(Consts.EnvironmentalVariableTagName))
            {
                foundEnvironmentVariables.Add(model.SecureVaultPass.GetBetweenParentheses());
            }

            return foundEnvironmentVariables;
        }

        private ConfigurationModel ProcessEnvironmentVariables(ConfigurationModel configuration, IDictionary<string, string> environmentVariables)
        {
            _objectValidator.IsNull(configuration, nameof(configuration));
            _objectValidator.IsNull(environmentVariables, nameof(environmentVariables));

            var newConfiguration = new ConfigurationModel
            {
                TemplatePath = EnvironmentVariableHelper.TryGetEnvironmentVariableValueForField(configuration.TemplatePath, environmentVariables),
                ResultPath = EnvironmentVariableHelper.TryGetEnvironmentVariableValueForField(configuration.ResultPath, environmentVariables),
                ProfilePath = EnvironmentVariableHelper.TryGetEnvironmentVariableValueForField(configuration.ProfilePath, environmentVariables),
                SecureVaultPath = EnvironmentVariableHelper.TryGetEnvironmentVariableValueForField(configuration.SecureVaultPath, environmentVariables),
                SecureVaultPass = EnvironmentVariableHelper.TryGetEnvironmentVariableValueForField(configuration.SecureVaultPass, environmentVariables),
            };

            return newConfiguration;
        }
    }
}
