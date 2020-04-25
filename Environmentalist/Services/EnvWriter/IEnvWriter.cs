using System.Threading.Tasks;
using Environmentalist.Models;

namespace Environmentalist.Services.EnvWriter
{
    public interface IEnvWriter
    {
        Task Write(TemplateModel model, string outputPath, string oroginalTemplatePath);
    }
}
