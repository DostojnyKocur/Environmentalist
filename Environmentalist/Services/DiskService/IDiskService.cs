using System.Threading.Tasks;

namespace Environmentalist.Services.DiskService
{
	public interface IDiskService
	{
		/// <summary>
		/// Asynchronously read content of the file 
		/// </summary>
		/// <param name="path">Path to file to read</param>
		/// <returns>Returns content of the file</returns>
		Task<string> ReadFileText(string path);

		/// <summary>
		/// Asynchronously writes given string to the file
		/// </summary>
		/// <param name="content">Content to write</param>
		/// <param name="path">Path to the file</param>
		Task WriteFileText(string content, string path);
	}
}