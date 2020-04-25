using System.Collections.Generic;
using Environmentalist.Models;

namespace Environmentalist.Services.LogicProcessor
{
    public interface ILogicProcessor
    {
        TemplateModel Process(TemplateModel template, ProfileModel profile, IDictionary<string, string> environmentVariables, ICollection<SecretEntryModel> secrets);
    }
}
