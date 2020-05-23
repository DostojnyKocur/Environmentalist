using System.Collections.Generic;
using Environmentalist.Models;

namespace Environmentalist.Services.LogicProcessor
{
    public interface ILogicProcessor
    {
        TemplateModel Process(TemplateModel template, ProfileModel profile, Pbkdf2Model protectedFile, ICollection<SecretEntryModel> secrets);
    }
}
