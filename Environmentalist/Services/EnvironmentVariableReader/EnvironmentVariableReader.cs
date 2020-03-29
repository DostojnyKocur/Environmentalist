using System;
using System.Collections.Generic;
using System.Linq;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;

namespace Environmentalist.Services.EnvironmentVariableReader
{
    public sealed class EnvironmentVariableReader : IEnvironmentVariableReader
    {    
        private readonly IStringValidator _stringValidator;
        private readonly IObjectValidator _objectValidator;

        public EnvironmentVariableReader(
            IStringValidator stringValidator,
            IObjectValidator objectValidator)
        {
            _stringValidator = stringValidator;
            _objectValidator = objectValidator;
        }

        public string Read(string variableName)
        {
            _stringValidator.IsNullOrWhitespace(variableName, nameof(variableName));

            return Environment.GetEnvironmentVariable(variableName) ?? string.Empty;
        }

        public IDictionary<string, string> Read(IEnumerable<string> variablesNames)
        {
            _objectValidator.IsNull(variablesNames, nameof(variablesNames));

            var readVariables = variablesNames.ToDictionary(item => item, item => Read(item));

            return readVariables;
        }
    }
}
