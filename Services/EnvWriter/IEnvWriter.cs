using System.Threading.Tasks;
using Environmentalist.Template;

namespace Environmentalist.Services.EnvWriter
{
    internal interface IEnvWriter
    {
        Task Write(TemplateModel model, string path);
    }
}
