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
    public class TransportationTaskModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("ImportanceLevel")]
        public int ImportanceLevel { get; set; }
        [BsonElement("Requester")]
        
        public string Requester { get; set; }
        [BsonElement("PUAddress")]


        public string PUAddress { get; set; }//startLocation
        [BsonElement("PUCity")]
        public string PUCity { get; set; }
        [BsonElement("PUPostal")]
        public string PUPostal { get; set; }
        [BsonElement("PUName")]
        public string PUName { get; set; }
        [BsonElement("PUDate"), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true), DataType(DataType.Date, ErrorMessage = "Date only")]
        public DateTime PUDate { get; set; }
        [BsonElement("PUTime")]
        public TimeSpan PUTime { get; set; }

        [BsonElement("PUAdditional")]
        public string PUAdditional { get; set; }

        [BsonElement("DOAddress")]
        public string DOAddress { get; set; }//endLocation
        [BsonElement("DOCity")]
        public string DOCity { get; set; }
        [BsonElement("DOPostal")]
        public string DOPostal { get; set; }
        [BsonElement("DOName")]
        public string DOName { get; set; }
        [BsonElement("DODate"), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true), DataType(DataType.Date, ErrorMessage = "Date only")]
        public DateTime DODate { get; set; }
        [BsonElement("DOTime")]
        public TimeSpan DOTime { get; set; }

        [BsonElement("DOAdditional")]
        public string DOAdditional { get; set; }

        [BsonElement("AdditionalInfo")]
        public string AdditionalInfo { get; set; }
        public class Item
        {
            public string ItemName { get; set; }
            public int ItemQuantity { get; set; }
            public string ItemNote { get; set; }

        }



       
        public List<Item> Items
        {
            get;
            set;

        }

        /** [BsonElement("ItemSample")]
        //fake item info
       public string ItemSample { get; set; }
        [BsonElement("QuantitySample")]
        public int QuantitySample { get; set; }
        [BsonElement("ItemNotesSample")]
        public string ItemNotesSample { get; set; }*/
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
}
}