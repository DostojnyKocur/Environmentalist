using System.Threading.Tasks;
using Environmentalist.Models;

namespace Environmentalist.Services.ProfileRepository
{
    public interface IProfileRepository
    {
        Task<ProfileModel> GetProfile(string path);
    }
}
