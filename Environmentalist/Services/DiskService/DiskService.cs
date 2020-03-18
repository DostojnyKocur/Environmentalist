using System.IO;
using System.Threading.Tasks;
using Environmentalist.Validators;

namespace Environmentalist.Services.DiskService
{
    internal sealed class DiskService : IDiskService
    {
        public async Task<string> ReadFileText(string path)
        {
            StringValidator.IsNullOrWhitespace(path, nameof(path));

            using (var reader = File.OpenText(path))
            {
                var fileText = await reader.ReadToEndAsync();
                return fileText;
            }
        }

        public async Task WriteFileText(string content, string path)
        {
            StringValidator.IsNullOrWhitespace(path, nameof(path));
            StringValidator.IsNullOrWhitespace(content, nameof(content));

            using (var writer = File.CreateText(path))
            {
                await writer.WriteAsync(content);
            }
        }
    }
}
