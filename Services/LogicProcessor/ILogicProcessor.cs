using Environmentalist.Models;

namespace Environmentalist.Services.LogicProcessor
{
    internal interface ILogicProcessor
    {
        TemplateModel Process(TemplateModel template, TemplateModel config);
    }
}
