using System.Collections.Generic;

namespace Environmentalist.Models
{
    public sealed class ConfigurationModel
    {
        public string TemplatePath { get; set; }
        public string ResultPath { get; set; }
        public string ConfigPath { get; set; }
        public string SecureVaultPath { get; set; }
        public string SecureVaultPass { get; set; }
    }
}
