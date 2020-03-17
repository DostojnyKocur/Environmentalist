using System.IO;
using System.Threading.Tasks;

namespace Environmentalist.Services.DiskService
{
    internal sealed class DiskService : IDiskService
    {
        public async Task<string> ReadFileText(string path)
        {
            using (var reader = File.OpenText(path))
            {
                var fileText = await reader.ReadToEndAsync();
                return fileText;
            }
        }

        public async Task WriteFileText(string content, string path)
        {
            using (var writer = File.CreateText(path))
            {
                await writer.WriteAsync(content);
            }
        }
    }
}
