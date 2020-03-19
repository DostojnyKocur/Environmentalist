using System;
using System.Text;
using System.Threading.Tasks;
using Environmentalist.Models;
using Environmentalist.Services.DiskService;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;

namespace Environmentalist.Services.EnvWriter
{
    public sealed class EnvWriter : IEnvWriter
    {
        private readonly IDiskService _diskService;
        private readonly IStringValidator _stringValidator;
        private readonly IObjectValidator _objectValidator;

        public EnvWriter(
            IDiskService diskService,
            IStringValidator stringValidator,
            IObjectValidator objectValidator)
        {
            _diskService = diskService;
            _stringValidator = stringValidator;
            _objectValidator = objectValidator;
        }

        public async Task Write(TemplateModel model, string path)
        {
            _stringValidator.IsNullOrWhitespace(path, nameof(path));
            _objectValidator.IsNull(model, nameof(model));

            var fileContent = new StringBuilder();
            foreach(var keyValue in model.Fields)
            {
                fileContent.Append(keyValue.Key);
                fileContent.Append('=');
                fileContent.Append(keyValue.Value);
                fileContent.Append(Environment.NewLine);
            }

            await _diskService.WriteFileText(fileContent.ToString(), path);
        }
    }
}
