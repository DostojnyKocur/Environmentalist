using System.Collections.Generic;
using System.Threading.Tasks;
using Environmentalist.Helpers;
using Environmentalist.Models;
using Environmentalist.Services.EnvironmentVariableReader;
using Environmentalist.Services.TemplateReader;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;

namespace Environmentalist.Services.TemplateRepository
{
    public sealed class TemplateRepository : ITemplateRepository
    {
        private readonly ITemplateReader _templateReader;
        private readonly IEnvironmentVariableReader _environmentVariableReader;
        private readonly IStringValidator _stringValidator;
        private readonly IObjectValidator _objectValidator;

        public TemplateRepository(
            ITemplateReader templateReader,
            IEnvironmentVariableReader environmentVariableReader,
            IStringValidator stringValidator,
            IObjectValidator objectValidator)
        {
            _templateReader = templateReader;
            _environmentVariableReader = environmentVariableReader;
            _stringValidator = stringValidator;
            _objectValidator = objectValidator;
        }

        public async Task<TemplateModel> GetTemplate(string path)
        {
            _stringValidator.IsNullOrWhitespace(path, nameof(path));

            var template = await _templateReader.Read(path);

            var environmentVariables = _templateReader.ExtractEnvironmentVariables(template);

            var environmentVariablesValues = _environmentVariableReader.Read(environmentVariables);

            template = ProcessEnvironmentVariables(template, environmentVariablesValues);

            return template;
        }

        private TemplateModel ProcessEnvironmentVariables(TemplateModel template, IDictionary<string, string> environmentVariables)
        {
            _objectValidator.IsNull(template, nameof(template));
            _objectValidator.IsNull(environmentVariables, nameof(environmentVariables));

            var newTamplate = new TemplateModel();

            foreach(var field in template.Fields)
            {
                var value = EnvironmentVariableHelper.TryGetEnvironmentVariableValueForField(field.Value, environmentVariables);
                newTamplate.Fields.Add(field.Key, value);
            }

            return newTamplate;
        }
    }
}
