using System.Collections.Generic;
using System.Threading.Tasks;
using Environmentalist.Models;

namespace Environmentalist.Services.Readers.TemplateReader
{
	public interface ITemplateReader
	{
		Task<TemplateModel> Read(string path);
		ICollection<string> ExtractEnvironmentVariables(TemplateModel model);
	}
}