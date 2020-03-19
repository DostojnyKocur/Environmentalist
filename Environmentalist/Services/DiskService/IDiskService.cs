using System.Threading.Tasks;

namespace Environmentalist.Services.DiskService
{
	public interface IDiskService
	{
		Task<string> ReadFileText(string path);
		Task WriteFileText(string content, string path);
	}
}