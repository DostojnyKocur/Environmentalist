using System.Collections.Generic;
using System.Threading.Tasks;
using Environmentalist.Models;

namespace Environmentalist.Services.ProfileReader
{
	public interface IProfileReader
	{
		Task<ProfileModel> Read(string path);
		ICollection<string> ExtractEnvironmentVariables(ProfileModel model);
	}
}