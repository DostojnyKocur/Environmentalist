using System;
using System.Threading.Tasks;
using Environmentalist.Models;
using Environmentalist.Services.DiskService;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;

namespace Environmentalist.Services.EnvWriter
{
    public sealed class EnvWriter : IEnvWriter
    {
        private const char AssignChar = '=';

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

        public async Task Write(TemplateModel model, string outputPath, string oroginalTemplatePath)
        {
            _stringValidator.IsNullOrWhitespace(outputPath, nameof(outputPath));
            _stringValidator.IsNullOrWhitespace(oroginalTemplatePath, nameof(oroginalTemplatePath));
            _objectValidator.IsNull(model, nameof(model));

            var orifinalFileContent = await _diskService.ReadFileText(oroginalTemplatePath);

            //This is stupid version of algorithm, should be improved in the future 
            foreach (var keyValue in model.Fields)
            {
                var indexOfKey = orifinalFileContent.IndexOf(keyValue.Key);
                var indexOfEndLine = orifinalFileContent.IndexOf(Environment.NewLine, indexOfKey);
                var charactersToCheck = indexOfEndLine - indexOfKey;
                var indexOfAssign = orifinalFileContent.IndexOf(AssignChar, indexOfKey, charactersToCheck);
                var indexOffirstCharacter = GetFirstNonWhitespace(orifinalFileContent, indexOfAssign, indexOfEndLine);
                var lengthToReplace = indexOfEndLine - indexOffirstCharacter;
                var partToReplace = orifinalFileContent.Substring(indexOffirstCharacter, lengthToReplace);
                orifinalFileContent = orifinalFileContent.Replace(partToReplace, keyValue.Value);
            }

            await _diskService.WriteFileText(orifinalFileContent, outputPath);
        }

        private int GetFirstNonWhitespace(string @string, int startIndex, int endIndex)
        {
            for (var i = startIndex + 1; i < endIndex; ++i)
            {
                if(!char.IsWhiteSpace(@string[i]))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
