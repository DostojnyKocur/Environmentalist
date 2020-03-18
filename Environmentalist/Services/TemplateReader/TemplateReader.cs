using System;
using System.Linq;
using System.Threading.Tasks;
using Environmentalist.Models;
using Environmentalist.Services.DiskService;
using Environmentalist.Validators;
using Environmentalist.Validators.FileValidator;

namespace Environmentalist.Services.TemplateReader
{
	internal sealed class TemplateReader : ITemplateReader
	{
		private readonly IDiskService _diskService;
		private readonly IFileValidator _fileValidator;

		public TemplateReader(
			IDiskService diskService,
			IFileValidator fileValidator)
		{
			_diskService = diskService;
			_fileValidator = fileValidator;
		}

		public async Task<TemplateModel> Read(string path)
		{
			StringValidator.IsNullOrWhitespace(path, nameof(path));
			_fileValidator.IsExist(path);

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