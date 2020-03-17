using System.Threading.Tasks;
using Environmentalist.Template;

namespace Environmentalist.Services.TemplateReader
{
	internal interface ITemplateReader
	{
		Task<TemplateModel> Read(string path);
	}
}