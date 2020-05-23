using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Environmentalist.Helpers;
using Environmentalist.Models;
using Environmentalist.Services.DiskService;
using Environmentalist.Services.Pbkdf2Service;
using Environmentalist.Services.Readers.EnvironmentVariableReader;
using Environmentalist.Validators.FileValidator;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;

namespace Environmentalist.Services.Repositories.Pbkdf2Repository
{
    public class Pbkdf2Repository : IPbkdf2Repository
    {
        private readonly IPbkdf2Service _pbkdF2Service;
        private readonly IEnvironmentVariableReader _environmentVariableReader;
        private readonly IFileSystem _fileSystem;
        private readonly IDiskService _diskService;
        private readonly IFileValidator _fileValidator;
        private readonly IStringValidator _stringValidator;
        private readonly IObjectValidator _objectValidator;

        public Pbkdf2Repository(
            IPbkdf2Service pbkdF2Service,
            IEnvironmentVariableReader environmentVariableReader,
            IFileSystem fileSystem,
            IDiskService diskService,
            IFileValidator fileValidator,
            IStringValidator stringValidator,
            IObjectValidator objectValidator)
        {
            _pbkdF2Service = pbkdF2Service;
            _environmentVariableReader = environmentVariableReader;
            _fileSystem = fileSystem;
            _diskService = diskService;
            _fileValidator = fileValidator;
            _stringValidator = stringValidator;
            _objectValidator = objectValidator;
        }

        public async Task Encrypt(string sourcePath, string outputPath)
        {
            _stringValidator.IsNullOrWhitespace(sourcePath, nameof(sourcePath));
            _stringValidator.IsNullOrWhitespace(outputPath, nameof(outputPath));
            var entropyPath = GetEntropyFilePath(outputPath);
            _fileValidator.IsExist(sourcePath);
            _fileValidator.IsNotExist(outputPath);
            _fileValidator.IsNotExist(entropyPath);

            var content = await _diskService.ReadFileText(sourcePath);

            var cipher = _pbkdF2Service.Encrypt(content);

            await _diskService.WriteFileText(cipher.cipherText, outputPath);
            await _diskService.WriteFileText(cipher.entropy, entropyPath);
        }

        public async Task<Pbkdf2Model> Decrypt(string path, string entropy)
        {
            _stringValidator.IsNullOrWhitespace(path, nameof(path));
            _stringValidator.IsNullOrWhitespace(entropy, nameof(entropy));
            _fileValidator.IsExist(path);

            var content = await _diskService.ReadFileText(path);
            var plaintext = _pbkdF2Service.Decrypt(content, entropy);

            var fields = ProcessFileHelper.ProcessContent(plaintext);

            var result = new Pbkdf2Model(fields);

            var environmentVariables = ProcessFileHelper.ExtractEnvironmentVariables(result.Fields);

            var environmentVariablesValues = _environmentVariableReader.Read(environmentVariables);

            result = ProcessEnvironmentVariables(result, environmentVariablesValues);

            return result;
        }

        private string GetEntropyFilePath(string outputPath)
        {
            var path = _fileSystem.Path.GetDirectoryName(outputPath);
            var fileName = _fileSystem.Path.GetFileNameWithoutExtension(outputPath);
            var extension = _fileSystem.Path.GetExtension(outputPath);

            var entropyFileName = $"{fileName}{Consts.EntropyFileSuffix}{extension}";

            var result = _fileSystem.Path.Combine(path, entropyFileName);

            return result;
        }

        private Pbkdf2Model ProcessEnvironmentVariables(Pbkdf2Model model, IDictionary<string, string> environmentVariables)
        {
            _objectValidator.IsNull(model, nameof(model));
            _objectValidator.IsNull(environmentVariables, nameof(environmentVariables));

            var newModel = new Pbkdf2Model();

            foreach (var field in model.Fields)
            {
                var value = EnvironmentVariableHelper.TryGetEnvironmentVariableValueForField(field.Value, environmentVariables);
                newModel.Fields.Add(field.Key, value);
            }

            return newModel;
        }
    }
}
