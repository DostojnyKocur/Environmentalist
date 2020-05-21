using System;
using System.Collections.Generic;
using System.Linq;
using Environmentalist.Helpers;
using NUnit.Framework;

namespace EnvironmentalistTests.Helpers
{
    [TestFixture]
    public class ProcessFileHelperTests
    {
        private const string Key1 = nameof(Key1);
        private const string Key2 = nameof(Key2);
        private const string KeyEnv1 = nameof(KeyEnv1);
        private const string KeyEnv2 = nameof(KeyEnv2);
        private const string Value1 = nameof(Value1);
        private const string Value2 = nameof(Value2);
        private const string ValueEnv1 = nameof(ValueEnv1);
        private const string ValueEnv2 = nameof(ValueEnv2);

        private static readonly string ValueEnv1Line = $"[EnvVar]({ValueEnv1})";
        private static readonly string ValueEnv2Line = $"[EnvVar]({ValueEnv2})";

        private static readonly string FileContent = @$"
                                    {Key1}={Value1}
                                    {Key2}={Value2}";

        private static readonly Dictionary<string, string> Fields = new Dictionary<string, string>
        {
            { Key1, Value1 },
            { Key2, Value2 },
            { KeyEnv1, ValueEnv1Line },
            { KeyEnv2, ValueEnv2Line }
        };

        [Test]
        public void When_process_content_and_content_is_null_Then_throws_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => ProcessFileHelper.ProcessContent(null));
        }

        [Test]
        public void When_process_content_Then_returns_found_fields()
        {
            var result = ProcessFileHelper.ProcessContent(FileContent);

            Assert.IsNotNull(result);
            Assert.AreEqual(Value1, result[Key1]);
            Assert.AreEqual(Value2, result[Key2]);
        }

        [Test]
        public void When_extract_environment_variables_And_parameter_is_null_Then_throws_argument_null_exception()
        {
            Assert.Throws<ArgumentNullException>(() => ProcessFileHelper.ExtractEnvironmentVariables(null));
        }

        [Test]
        public void When_extract_environment_variables_Then_returns_list_of_environment_variables()
        {
            var result = ProcessFileHelper.ExtractEnvironmentVariables(Fields).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(ValueEnv1, result[0]);
            Assert.AreEqual(ValueEnv2, result[1]);
        }
    }
}
