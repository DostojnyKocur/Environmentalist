using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Environmentalist.Extensions;
using Environmentalist.Models;
using Environmentalist.Services.DiskService;
using Environmentalist.Validators.FileValidator;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;
using Serilog;

namespace Environmentalist.Services.ConfigurationReader
{
    public sealed class ConfigurationReader : IConfigurationReader
    {
        private readonly IDiskService _diskService;
        private readonly IFileValidator _fileValidator;
        private readonly IStringValidator _stringValidator;
        private readonly IObjectValidator _objectValidator;

        public ConfigurationReader(
            IDiskService diskService,
            IFileValidator fileValidator,
            IStringValidator stringValidator,
            IObjectValidator objectValidator)
        {
            _diskService = diskService;
            _fileValidator = fileValidator;
            _stringValidator = stringValidator;
            _objectValidator = objectValidator;
        }

        public async Task<ConfigurationModel> Read(string path)
        {
            _stringValidator.IsNullOrWhitespace(path, nameof(path));
            _fileValidator.IsExist(path);

            var fileContent = await _diskService.ReadFileText(path);
            var lines = fileContent.Split(Environment.NewLine).ToList();

            var result = new ConfigurationModel();

            lines.ForEach(line =>
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    AssignConfigurationValue(result, line);
                }
            });

            return result;
        }

        public ICollection<string> ExtractEnvironmentVariables(ConfigurationModel model)
        {
            var foundEnvironmentVariables = new List<string>();
                
            if(model.TemplatePath.StartsWith(Consts.EnvironmentalVariableTagName))
            {
                foundEnvironmentVariables.Add(model.TemplatePath.GetBetweenParentheses());
            }
            if (model.ResultPath.StartsWith(Consts.EnvironmentalVariableTagName))
            {
                foundEnvironmentVariables.Add(model.ResultPath.GetBetweenParentheses());
            }
            if (model.ConfigPath.StartsWith(Consts.EnvironmentalVariableTagName))
            {
                foundEnvironmentVariables.Add(model.ConfigPath.GetBetweenParentheses());
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

        public ConfigurationModel ProcessEnvironmentVariables(ConfigurationModel configuration, IDictionary<string, string> environmentVariables)
        {
            _objectValidator.IsNull(environmentVariables, nameof(environmentVariables));

            var newConfiguration = new ConfigurationModel
            {
                TemplatePath = TryGetEnvironmentVariableValue(configuration.TemplatePath, environmentVariables),
                ResultPath = TryGetEnvironmentVariableValue(configuration.ResultPath, environmentVariables),
                ConfigPath = TryGetEnvironmentVariableValue(configuration.ConfigPath, environmentVariables),
                SecureVaultPath = TryGetEnvironmentVariableValue(configuration.SecureVaultPath, environmentVariables),
                SecureVaultPass = TryGetEnvironmentVariableValue(configuration.SecureVaultPass, environmentVariables),
            };

            return newConfiguration;
        }

        private static string TryGetEnvironmentVariableValue(string field, IDictionary<string, string> environmentVariables)
        {
            if(!field.StartsWith(Consts.EnvironmentalVariableTagName))
            {
                return field;
            }

            if (!environmentVariables.Any())
            {
                throw new InvalidOperationException("No environment variables values found");
            }

            try
            {
                var environmentVariableName = field.GetBetweenParentheses();
                var environmentVariableValue = environmentVariables[environmentVariableName];
                return environmentVariableValue;
            }
            catch (KeyNotFoundException exception)
            {
                Log.Logger.Error(exception, $"Environment variable value for '{field}' has not been found");
                throw;
            }
        }

        private static void AssignConfigurationValue(ConfigurationModel config, string configLine)
        {
            var keyValue = configLine.Split('=');
            var key = keyValue[0].Trim().ToLower();
            var value = keyValue[1].Trim();

            switch (key)
            {
                case "templatepath":
                    config.TemplatePath = value;
                    break;
                case "resultpath":
                    config.ResultPath = value;
                    break;
                case "configpath":
                    config.ConfigPath = value;
                    break;
                case "securevaultpath":
                    config.SecureVaultPath = value;
                    break;
                case "securevaultpass":
                    config.SecureVaultPass = value;
                    break;
                default:
                    Log.Logger.Warning($"Unknown configuration key '{key}'. Value will be ignored");
                    break;
            }
        }
    }
}
