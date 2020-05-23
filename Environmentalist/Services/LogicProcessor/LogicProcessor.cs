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

        public TemplateModel Process(TemplateModel template, ProfileModel profile, Pbkdf2Model protectedFile, ICollection<SecretEntryModel> secrets)
        {
            _objectValidator.IsNull(template, nameof(template));
            _objectValidator.IsNull(profile, nameof(profile));
            _objectValidator.IsNull(protectedFile, nameof(protectedFile));
            _objectValidator.IsNull(secrets, nameof(secrets));

            var resultModel = new TemplateModel();

            try
            {
                foreach (var templateLine in template.Fields)
                {
                    var key = templateLine.Key;
                    var value = TryGetValue(templateLine.Value, template, profile, protectedFile, secrets);

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

        private static string TryGetValue(string value, TemplateModel template, ProfileModel profile, Pbkdf2Model protectedFile, ICollection<SecretEntryModel> secrets)
        {
            var foundValue = profile.Fields.ContainsKey(value)
                ? profile.Fields[value]
                : (protectedFile.Fields.ContainsKey(value)
                ? protectedFile.Fields[value]
                : value);

            foundValue = TryGetCustomValue(foundValue, secrets);

            return foundValue;
        }

        private static string TryGetCustomValue(string value, ICollection<SecretEntryModel> secrets)
        {
            if (value.StartsWith(Consts.KeePassTagName))
            {
                return TryReturnsSecret(value, secrets);
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
    }
}
