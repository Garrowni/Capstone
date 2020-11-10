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
    public class InventoryTaskModel
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



        [BsonElement("address")]
        public string address { get; set; }
        [BsonElement("taskDate"), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true), DataType(DataType.Date, ErrorMessage = "Date only")]
        public DateTime taskDate { get; set; }
        [BsonElement("taskTime")]
        public TimeSpan taskTime { get; set; }


        public string AdditionalInfo { get; set; }



        // public class documents
        // {
        // public HttpPostedFileBase Document { get; set; }  //not sure how to do this yet
        public string File { get; set; }

        // }

        public List<string> FileList { get; set; }




    }
}