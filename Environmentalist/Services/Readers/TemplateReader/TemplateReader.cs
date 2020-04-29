using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Environmentalist.Extensions;
using Environmentalist.Models;
using Environmentalist.Services.DiskService;
using Environmentalist.Validators.FileValidator;
using Environmentalist.Validators.ObjectValidator;
using Environmentalist.Validators.StringValidator;

namespace Environmentalist.Services.Readers.TemplateReader
{
	public sealed class TemplateReader : ITemplateReader
	{
		private readonly IDiskService _diskService;
		private readonly IFileValidator _fileValidator;
		private readonly IStringValidator _stringValidator;
		private readonly IObjectValidator _objectValidator;

		public TemplateReader(
			IDiskService diskService,
			IFileValidator fileValidator,
			IStringValidator stringValidator,
			IObjectValidator objectValidator)
		{
			_diskService = diskService;
			_fileValidator = fileValidator;
			_stringValidator = stringValidator;
			_objectValidator = objectValidator;
		}

		public async Task<TemplateModel> Read(string path)
		{
			_stringValidator.IsNullOrWhitespace(path, nameof(path));
			_fileValidator.IsExist(path);

			var fileContent = await _diskService.ReadFileText(path);
			var lines = fileContent.Split(Environment.NewLine).ToList();

			var result = new TemplateModel();

			lines.ForEach(line =>
			{
				if (!string.IsNullOrWhiteSpace(line))
				{
					var keyValue = line.Split('=');
					result.Fields.Add(keyValue[0].Trim(), keyValue[1].Trim());
				}
			});

			return result;
		}

		public ICollection<string> ExtractEnvironmentVariables(TemplateModel model)
		{
			_objectValidator.IsNull(model, nameof(model));

			var foundEnvironmentVariables = model.Fields
				.Where(keyPair => keyPair.Value.StartsWith(Consts.EnvironmentalVariableTagName))
				.Select(keyPair => keyPair.Value.GetBetweenParentheses())
				.ToList();

			return foundEnvironmentVariables;
		}
	}
}