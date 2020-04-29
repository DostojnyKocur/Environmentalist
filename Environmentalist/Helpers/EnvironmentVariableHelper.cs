using System;
using System.Collections.Generic;
using System.Linq;
using Environmentalist.Extensions;
using Serilog;

namespace Environmentalist.Helpers
{
    public static class EnvironmentVariableHelper
    {
        public static string TryGetEnvironmentVariableValueForField(string field, IDictionary<string, string> environmentVariables)
        {
            if (!field.StartsWith(Consts.EnvironmentalVariableTagName))
            {
                return field;
            }

            if (environmentVariables is null || !environmentVariables.Any())
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
    }
}
