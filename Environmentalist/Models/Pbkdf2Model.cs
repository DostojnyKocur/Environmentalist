using System.Collections.Generic;

namespace Environmentalist.Models
{
	public sealed class Pbkdf2Model
	{
		public Pbkdf2Model() { }
		public Pbkdf2Model(Dictionary<string, string> initialData) => Fields = initialData;

		public Dictionary<string, string> Fields { get; } = new Dictionary<string, string>();
	}
}