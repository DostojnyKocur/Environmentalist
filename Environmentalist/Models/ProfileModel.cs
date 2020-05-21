using System.Collections.Generic;

namespace Environmentalist.Models
{
	public sealed class ProfileModel
	{
		public ProfileModel() { }
		public ProfileModel(Dictionary<string, string> initialData) => Fields = initialData;

		public Dictionary<string, string> Fields { get; } = new Dictionary<string, string>();
	}
}