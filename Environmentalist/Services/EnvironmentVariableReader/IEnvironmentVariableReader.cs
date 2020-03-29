using System.Collections.Generic;

namespace Environmentalist.Services.EnvironmentVariableReader
{
    public interface IEnvironmentVariableReader
    {
        string Read(string variableName);
        IDictionary<string, string> Read(IEnumerable<string> variablesNames);
    }
}
