using System.Collections.Generic;

namespace Environmentalist.Models
{
	public sealed class ProfileModel
	{
		public Dictionary<string, string> Fields { get; } = new Dictionary<string, string>();
	}
}