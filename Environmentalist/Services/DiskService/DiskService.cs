using System.IO.Abstractions;
using System.Threading.Tasks;
using Environmentalist.Validators;

namespace Environmentalist.Services.DiskService
{
    public sealed class DiskService : IDiskService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IValidators _validators;

        public DiskService(
            IFileSystem fileSystem,
            IValidators validators)
        {
            _fileSystem = fileSystem;
            _validators = validators;

        }

        public async Task<string> ReadFileText(string path)
        {
            _validators.StringValidator.IsNullOrWhitespace(path, nameof(path));
            _validators.FileValidator.IsExist(path);

            using (var reader = _fileSystem.File.OpenText(path))
            {
                var fileText = await reader.ReadToEndAsync();
                return fileText;
            }
        }

        public async Task WriteFileText(string content, string path)
        {
            _validators.StringValidator.IsNullOrWhitespace(path, nameof(path));
            _validators.StringValidator.IsNullOrWhitespace(content, nameof(content));

            using (var writer = _fileSystem.File.CreateText(path))
            {
                await writer.WriteAsync(content);
            }
        }
    }
}
