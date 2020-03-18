using System.Collections.Generic;

namespace Environmentalist.Models
{
	internal sealed class TemplateModel
	{
		public Dictionary<string, string> Fields { get; } = new Dictionary<string, string>();
	}
}