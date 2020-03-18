using System;
using System.Linq;
using System.Threading.Tasks;
using Environmentalist.Models;
using Environmentalist.Services.DiskService;
using Environmentalist.Validators;

namespace Environmentalist.Services.TemplateReader
{
	internal sealed class TemplateReader : ITemplateReader
	{
		private readonly IDiskService _diskService;

		public TemplateReader(IDiskService diskService)
		{
			_diskService = diskService;
		}

		public async Task<TemplateModel> Read(string path)
		{
			StringValidator.IsNullOrWhitespace(path, nameof(path));
			FileValidator.IsExist(path);

			var fileContent = await _diskService.ReadFileText(path);
			var lines = fileContent.Split(Environment.NewLine).ToList();

			var result = new TemplateModel();

			lines.ForEach(line =>
			{
				if (!string.IsNullOrWhiteSpace(line))
				{
					var keyValue = line.Split('=');
					result.Fields.Add(keyValue[0], keyValue[1]);
				}
			});

			return result;
		}
	}
}