﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecilStoreProject.Entities.Interfaces;

public abstract class DatabaseSettings
{
	public string ConnectionString { get; set; }
	public string DatabaseName { get; set; }
	public string CollectionName { get; set; }
}
