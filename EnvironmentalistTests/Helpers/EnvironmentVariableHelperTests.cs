using System;
using System.Collections.Generic;
using Environmentalist.Helpers;
using NUnit.Framework;

namespace EnvironmentalistTests.Helpers
{
    [TestFixture]
    public class EnvironmentVariableHelperTests
    {
        private const string VariableName1 = nameof(VariableName1);
        private const string VariableName2 = nameof(VariableName2);
        private const string VariableName3 = nameof(VariableName3);
        private const string VariableValue1 = nameof(VariableValue1);
        private const string VariableValue2 = nameof(VariableValue2);

        private const string InvalidEnvName = nameof(InvalidEnvName);

        private static readonly string Variable1Line = $"[EnvVar]({VariableName1})";
        private static readonly string Variable2Line = $"[EnvVar]({VariableName2})";
        private static readonly string Variable3Line = $"[EnvVar]({VariableName3})";

        private static readonly Dictionary<string, string> EnvVarValues = new Dictionary<string, string>
        {
            { VariableName1, VariableValue1 },
            { VariableName2, VariableValue2 },
        };

        [Test]
        public void When_try_get_environment_variable_value_for_field_Then_returns_found_variable()
        {
            var result = EnvironmentVariableHelper.TryGetEnvironmentVariableValueForField(Variable2Line, EnvVarValues);

            Assert.AreEqual(VariableValue2, result);
        }

        [Test]
        public void When_try_get_environment_variable_value_for_field_And_field_is_not_env_var_Then_returns_field()
        {
            var result = EnvironmentVariableHelper.TryGetEnvironmentVariableValueForField(InvalidEnvName, EnvVarValues);

            Assert.AreEqual(InvalidEnvName, result);
        }

        [Test]
        public void When_try_get_environment_variable_value_for_field_And_env_var_dictionary_is_empty_Then_throws_invalid_operation_exception()
        {
            Assert.Throws<InvalidOperationException>(() => EnvironmentVariableHelper.TryGetEnvironmentVariableValueForField(Variable1Line, new Dictionary<string, string>()));
        }

        [Test]
        public void When_try_get_environment_variable_value_for_field_And_env_var_dictionary_is_null_Then_throws_invalid_operation_exception()
        {
            Assert.Throws<InvalidOperationException>(() => EnvironmentVariableHelper.TryGetEnvironmentVariableValueForField(Variable1Line, null));
        }

        [Test]
        public void When_try_get_environment_variable_value_for_field_And_field_is_not_present_Then_throws_key_not_found_rxception()
        {
            Assert.Throws<KeyNotFoundException>(() => EnvironmentVariableHelper.TryGetEnvironmentVariableValueForField(Variable3Line, EnvVarValues));
        }
    }
}
