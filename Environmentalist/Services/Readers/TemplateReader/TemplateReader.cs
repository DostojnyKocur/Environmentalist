using System.Collections.Generic;
using System.Threading.Tasks;
using Environmentalist.Helpers;
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
			var fields = ProcessFileHelper.ProcessContent(fileContent);

			var result = new TemplateModel(fields);

			return result;
		}

		public ICollection<string> ExtractEnvironmentVariables(TemplateModel model)
		{
			_objectValidator.IsNull(model, nameof(model));

			var foundEnvironmentVariables = ProcessFileHelper.ExtractEnvironmentVariables(model.Fields);

			return foundEnvironmentVariables;
		}
	}
}