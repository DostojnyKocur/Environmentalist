using System.Collections.Generic;

namespace Environmentalist.Models
{
	public sealed class TemplateModel
	{
		public TemplateModel() { }
		public TemplateModel(Dictionary<string, string> initialData) => Fields = initialData;

		public Dictionary<string, string> Fields { get; } = new Dictionary<string, string>();
	}
}