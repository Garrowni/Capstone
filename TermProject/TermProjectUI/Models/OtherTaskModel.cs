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
        [BsonElement("requester")]
        public string requester { get; set; }
        [BsonElement("reqPhoto")]
        public string reqPhoto { get; set; }
        [BsonElement("taskTitle")]
        public string taskTitle { get; set; }
        [BsonElement("posterName")]
        public string posterName { get; set; }
        
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
        [BsonElement("ImportanceLevel")]
        public int ImportanceLevel { get; set; }
        [BsonElement("assignees")]
        public List<string> assignees { get; set; }
        public string AdditionalInfo { get; set; }
        [BsonElement("posterPhoto")]
        public string posterPhoto { get; set; }
        [BsonElement("state")]

        public string state { get; set; }
    }
}