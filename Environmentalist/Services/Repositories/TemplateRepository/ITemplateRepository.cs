using System.Threading.Tasks;
using Environmentalist.Models;

namespace Environmentalist.Services.Repositories.TemplateRepository
{
    public interface ITemplateRepository
    {
        Task<TemplateModel> GetTemplate(string path);
    }
}
