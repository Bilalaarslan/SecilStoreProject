﻿using SecilStoreProject.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecilStoreProject.Interface.Repositories;

public interface IConfigurationRepository
{
    Task<IEnumerable<ConfigurationModel>> GetAllConfigurationsAsync(string applicationName);
    Task<ConfigurationModel> GetConfigurationByIdAsync(string id);
    Task CreateConfigurationAsync(ConfigurationModel configurationModel);
    Task<bool> UpdateConfigurationAsync(ConfigurationModel configurationModel);
    Task<bool> DeleteAsync(string id);

}
