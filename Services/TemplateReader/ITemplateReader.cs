using System.Threading.Tasks;
using Environmentalist.Models;

namespace Environmentalist.Services.TemplateReader
{
	internal interface ITemplateReader
	{
		Task<TemplateModel> Read(string path);
	}
}