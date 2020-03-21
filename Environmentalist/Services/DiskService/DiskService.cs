using System.IO.Abstractions;
using System.Threading.Tasks;
using Environmentalist.Validators.FileValidator;
using Environmentalist.Validators.StringValidator;

namespace Environmentalist.Services.DiskService
{
    public sealed class DiskService : IDiskService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IFileValidator _fileValidator;
        private readonly IStringValidator _stringValidator;

        public DiskService(
            IFileSystem fileSystem,
            IFileValidator fileValidator,
            IStringValidator stringValidator)
        {
            _fileSystem = fileSystem;
            _fileValidator = fileValidator;
            _stringValidator = stringValidator;

        }

        public async Task<string> ReadFileText(string path)
        {
            _stringValidator.IsNullOrWhitespace(path, nameof(path));
            _fileValidator.IsExist(path);

            using (var reader = _fileSystem.File.OpenText(path))
            {
                var fileText = await reader.ReadToEndAsync();
                return fileText;
            }
        }

        public async Task WriteFileText(string content, string path)
        {
            _stringValidator.IsNullOrWhitespace(path, nameof(path));
            _stringValidator.IsNullOrWhitespace(content, nameof(content));

            using (var writer = _fileSystem.File.CreateText(path))
            {
                await writer.WriteAsync(content);
            }
        }
    }
}
