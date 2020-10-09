using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TermProjectUI.Models
{
    public class PhotographyTaskModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        
       
  

        [BsonElement("taskID")]
        //auto generate
        public int taskID { get; set; }
        [BsonElement("taskName")]
        public string taskName { get; set; }
        [BsonElement("taskType")]
        public string taskType { get; set; }
        [BsonElement("posterName")]
        public string posterName { get; set; }
        [BsonElement("assignees")]
        public List<string> assignees { get; set; }
        [BsonElement("creationDate")]
        public DateTime creationDate { get; set; }
        [BsonElement("state")]
        public string state { get; set; }



        [BsonElement("location")]
        public string location { get; set; }
        [BsonElement("photographerName")]
        public string photographerName { get; set; }
        [BsonElement("date")]
        public DateTime date { get; set; }
        [BsonElement("time")]
        public DateTime time{ get; set; }



        public class dog
        {
            public string Name { get; set; }
            public string Breed { get; set; }
            public string Size { get; set; }
            public string Transportation { get; set; }
            public string transporter { get; set; }
            public string notes { get; set; }
        }

        public List<dog> Dogs
        {
            get;
            set;
        }

        [BsonElement("AdditionalInfo")]
        public string AdditionalInfo { get; set; }

    }
}