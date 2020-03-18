using System;
using System.Text;
using System.Threading.Tasks;
using Environmentalist.Models;
using Environmentalist.Services.DiskService;
using Environmentalist.Validators;

namespace Environmentalist.Services.EnvWriter
{
    internal sealed class EnvWriter : IEnvWriter
    {
        private readonly IDiskService _diskService;

        public EnvWriter(IDiskService diskService)
        {
            _diskService = diskService;
        }

        public async Task Write(TemplateModel model, string path)
        {
            StringValidator.IsNullOrWhitespace(path, nameof(path));

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
