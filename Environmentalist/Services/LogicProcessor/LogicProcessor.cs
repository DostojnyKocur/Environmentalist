using System;
using System.Collections.Generic;
using System.Linq;
using Environmentalist.Extensions;
using Environmentalist.Models;
using Environmentalist.Validators.ObjectValidator;
using Serilog;

namespace Environmentalist.Services.LogicProcessor
{
    public sealed class LogicProcessor : ILogicProcessor
    {
        private readonly IObjectValidator _objectValidator;

        public LogicProcessor(IObjectValidator objectValidator)
        {
            _objectValidator = objectValidator;
        }

        public TemplateModel Process(TemplateModel template, ProfileModel profile, IDictionary<string, string> environmentVariables, ICollection<SecretEntryModel> secrets)
        {
            _objectValidator.IsNull(template, nameof(template));
            _objectValidator.IsNull(profile, nameof(profile));
            _objectValidator.IsNull(environmentVariables, nameof(environmentVariables));
            _objectValidator.IsNull(secrets, nameof(secrets));

            var resultModel = new TemplateModel();

            try
            {
                foreach (var templateLine in template.Fields)
                {
                    var key = templateLine.Key;
                    var value = profile.Fields.ContainsKey(templateLine.Value) ? profile.Fields[templateLine.Value] : templateLine.Value;

                    value = TryGetCustomValue(value, environmentVariables, secrets);

                    resultModel.Fields.Add(key, value);

                    Log.Logger.Debug($"Bounded key: '{key}' to value '{value}'");
                }
            }
            catch (Exception exception)
            {
                Log.Logger.Error(exception, "During preparing output an error has been occured");
                throw;
            }

            return resultModel;
        }

        private static string TryGetCustomValue(string value, IDictionary<string, string> environmentVariables, ICollection<SecretEntryModel> secrets)
        {
            if (value.StartsWith(Consts.KeePassTagName))
            {
                return TryReturnsSecret(value, secrets);
            }
            else if (value.StartsWith(Consts.EnvironmentalVariableTagName))
            {
                return TryReturnEnvironmentVariable(value, environmentVariables);
            }

            return value;
        }

        private static string TryReturnsSecret(string value, ICollection<SecretEntryModel> secrets)
        {
            if (!secrets.Any())
            {
                throw new InvalidOperationException("No secrets found");
            }

            try
            {
                var secretType = value.GetBetweenParentheses();
                var secret = secrets.First(entry => entry.Title.Equals(secretType) || entry.UserName.Equals(secretType));
                return secret.Password;
            }
            catch (IndexOutOfRangeException exception)
            {
                Log.Logger.Error(exception, $"During parsing configuration value '{value}' an error has been occured");
                throw;
            }
            catch (InvalidOperationException exception)
            {
                Log.Logger.Error(exception, $"Secret '{value}' has not been found");
                throw;
            }
        }

        private static string TryReturnEnvironmentVariable(string value, IDictionary<string, string> environmentVariables)
        {
            if (!environmentVariables.Any())
            {
                throw new InvalidOperationException("No environment variables values found");
            }

            try
            {
                var environmentVariableName = value.GetBetweenParentheses();
                var environmentVariableValue = environmentVariables[environmentVariableName];
                return environmentVariableValue;
            }
            catch (IndexOutOfRangeException exception)
            {
                Log.Logger.Error(exception, $"During parsing configuration value '{value}' an error has been occured");
                throw;
            }
            catch (KeyNotFoundException exception)
            {
                Log.Logger.Error(exception, $"Environment variable value for '{value}' has not been found");
                throw;
            }
        }
    }
}
