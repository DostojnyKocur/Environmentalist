using System;
using System.Linq;
using System.Threading.Tasks;
using Environmentalist.Models;
using Environmentalist.Services.DiskService;
using Environmentalist.Validators.FileValidator;
using Environmentalist.Validators.StringValidator;
using Serilog;

namespace Environmentalist.Services.ConfigurationReader
{
    public sealed class ConfigurationReader : IConfigurationReader
    {
        private readonly IDiskService _diskService;
        private readonly IFileValidator _fileValidator;
        private readonly IStringValidator _stringValidator;

        public ConfigurationReader(
            IDiskService diskService,
            IFileValidator fileValidator,
            IStringValidator stringValidator)
        {
            _diskService = diskService;
            _fileValidator = fileValidator;
            _stringValidator = stringValidator;
        }

        public async Task<ConfigurationModel> Read(string path)
        {
            _stringValidator.IsNullOrWhitespace(path, nameof(path));
            _fileValidator.IsExist(path);

            var fileContent = await _diskService.ReadFileText(path);
            var lines = fileContent.Split(Environment.NewLine).ToList();

            var result = new ConfigurationModel();

            lines.ForEach(line =>
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    AssignConfigurationValue(result, line);
                }
            });

            return result;
        }

        private static void AssignConfigurationValue(ConfigurationModel config, string configLine)
        {
            var keyValue = configLine.Split('=');
            var key = keyValue[0].Trim().ToLower();
            var value = keyValue[1].Trim();

            switch (key)
            {
                case "templatepath":
                    config.TemplatePath = value;
                    break;
                case "resultpath":
                    config.ResultPath = value;
                    break;
                case "profilepath":
                    config.ProfilePath = value;
                    break;
                case "securevaultpath":
                    config.SecureVaultPath = value;
                    break;
                case "securevaultpass":
                    config.SecureVaultPass = value;
                    break;
                default:
                    Log.Logger.Warning($"Unknown configuration key '{key}'. Value will be ignored");
                    break;
            }
        }
    }
}
