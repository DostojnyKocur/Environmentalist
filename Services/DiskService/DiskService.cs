using System.IO;
using System.Threading.Tasks;

namespace Environmentalist.Services.DiskService
{
    internal class DiskService : IDiskService
    {
        public async Task<string> ReadFileText(string path)
        {
            using (var reader = File.OpenText(path))
            {
                var fileText = await reader.ReadToEndAsync();
                return fileText;
            }
        }
    }
}
