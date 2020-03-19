using Environmentalist.Models;
using Environmentalist.Validators.ObjectValidator;
using Serilog;

namespace Environmentalist.Services.LogicProcessor
{
    public sealed class LogicProcessor : ILogicProcessor
    {
        private readonly IObjectValidator _objectValidator;

        public LogicProcessor(IObjectValidator objectValidator)
        {
            _objectValidator = objectValidator;
        }

        public TemplateModel Process(TemplateModel template, TemplateModel config)
        {
            _objectValidator.IsNull(template, nameof(template));
            _objectValidator.IsNull(config, nameof(config));

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
