using System;
using System.Collections.Generic;
using System.Linq;
using Environmentalist.Extensions;

namespace Environmentalist.Helpers
{
    public static class ProcessFileHelper
    {
        public static Dictionary<string, string> ProcessContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException(nameof(content));
            }

            var result = new Dictionary<string, string>();
            var lines = content.Split(Environment.NewLine).ToList();

            lines.ForEach(line =>
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var keyValue = line.Split('=');
                    result.Add(keyValue[0].Trim(), keyValue[1].Trim());
                }
            });

            return result;
        }

        public static ICollection<string> ExtractEnvironmentVariables(IDictionary<string, string> fields)
        {
            if (fields is null || !fields.Any())
            {
                throw new ArgumentNullException(nameof(fields));
            }

            var foundEnvironmentVariables = fields
                .Where(keyPair => keyPair.Value.StartsWith(Consts.EnvironmentalVariableTagName))
                .Select(keyPair => keyPair.Value.GetBetweenParentheses())
                .ToList();

            return foundEnvironmentVariables;
        }
    }
}
