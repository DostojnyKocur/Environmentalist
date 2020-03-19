using System.Threading.Tasks;
using Environmentalist.Models;

namespace Environmentalist.Services.TemplateReader
{
	public interface ITemplateReader
	{
		Task<TemplateModel> Read(string path);
	}
}