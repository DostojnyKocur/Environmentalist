﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Environmentalist.Models;

namespace Environmentalist.Services.ConfigurationReader
{
    public interface IConfigurationReader
    {
        Task<ConfigurationModel> Read(string path);
        ICollection<string> ExtractEnvironmentVariables(ConfigurationModel model);
        ConfigurationModel ProcessEnvironmentVariables(ConfigurationModel configuration, IDictionary<string, string> environmentVariables);
    }
}
