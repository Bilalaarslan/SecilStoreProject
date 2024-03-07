using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SecilStoreProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecilStoreProject.Entities.Entities;

public class ConfigurationModel
{
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string Id { get; set; }
	public string Name { get; set; }
	public ConfigurationValueType Type { get; set; }
	public string Value { get; set; }
	public bool IsActive { get; set; }
	public string ApplicationName { get; set; }

}
