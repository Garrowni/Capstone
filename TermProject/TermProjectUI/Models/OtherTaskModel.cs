using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TermProjectUI.Models
{
    public class OtherTaskModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("taskName")]
        public string taskName { get; set; }
        public class TaskRequirement
        {
            public string Key { get; set; }
            public string Value { get; set; }

        }




        public List<TaskRequirement> TaskRequirements
        {
            get;
            set;

        }

    }
}