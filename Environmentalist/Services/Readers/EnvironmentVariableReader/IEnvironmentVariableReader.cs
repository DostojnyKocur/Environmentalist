using System.Collections.Generic;

namespace Environmentalist.Services.Readers.EnvironmentVariableReader
{
    public interface IEnvironmentVariableReader
    {
        string Read(string variableName);
        IDictionary<string, string> Read(IEnumerable<string> variablesNames);
    }
}
