using System.Collections.Generic;

namespace Environmentalist.Template
{
	internal sealed class TemplateModel
	{
		public Dictionary<string, string> Fields { get; } = new Dictionary<string, string>();
	}
}