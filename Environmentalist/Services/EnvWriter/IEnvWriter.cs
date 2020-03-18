using System.Threading.Tasks;
using Environmentalist.Models;

namespace Environmentalist.Services.EnvWriter
{
    internal interface IEnvWriter
    {
        Task Write(TemplateModel model, string path);
    }
}
