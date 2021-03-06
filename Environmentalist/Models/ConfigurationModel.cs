﻿namespace Environmentalist.Models
{
    public sealed class ConfigurationModel
    {
        public string TemplatePath { get; set; }
        public string ResultPath { get; set; }
        public string ProfilePath { get; set; }
        public string SecureVaultPath { get; set; }
        public string SecureVaultPass { get; set; }
        public string ProtectedFilePath { get; set; }
        public string ProtectedFileEntropy { get; set; }
    }
}
