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



        [BsonElement("posterPhoto")]
        public string posterPhoto { get; set; }

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


        [BsonElement("requester")]
        public string requester { get; set; }
        [BsonElement("reqPhoto")]
        public string reqPhoto { get; set; }

        [BsonElement("location")]
        public string location { get; set; }
        [BsonElement("photographerName")]
        public string photographerName { get; set; }
        [BsonElement("taskDate"), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true), DataType(DataType.Date, ErrorMessage = "Date only")]
        public DateTime taskDate { get; set; }
        [BsonElement("taskTime")]
        public TimeSpan taskTime{ get; set; }



        public class dog
        {
            public string txtName { get; set; }
            public string txtBreed { get; set; }
            public string txtSize { get; set; }

            public string txtNotes { get; set; }
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