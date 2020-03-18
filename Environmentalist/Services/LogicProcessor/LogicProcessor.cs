using Environmentalist.Models;
using Environmentalist.Validators;
using Serilog;

namespace Environmentalist.Services.LogicProcessor
{
    internal sealed class LogicProcessor : ILogicProcessor
    {
        public TemplateModel Process(TemplateModel template, TemplateModel config)
        {
            ObjectValidator.IsNull(template, nameof(template));
            ObjectValidator.IsNull(config, nameof(config));

            var resultModel = new TemplateModel();

            foreach(var templateLine in template.Fields)
            {
                var key = templateLine.Key;
                var value = config.Fields.ContainsKey(templateLine.Value) ? config.Fields[templateLine.Value] : templateLine.Value;

                resultModel.Fields.Add(key, value);

                Log.Logger.Debug($"Bounded key: '{key}' to value '{value}'");
            }

            return resultModel;
        }
    }
}
