using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TermProjectUI.Models
{
    public class DeletedTaskModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("taskType")]
        public string tasksType{ get; set; }
        public List<Object> deletedTask
        {
            get;
            set;

        }

    }
}