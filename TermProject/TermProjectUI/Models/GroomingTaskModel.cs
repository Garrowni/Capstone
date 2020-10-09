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
    public class GroomingTaskModel
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



        //task spec
        [BsonElement("dogName")]
        public string dogName { get; set; }
        [BsonElement("dogAge")]
        public int dogAge { get; set; }
        [BsonElement("dogBreed")]
        public string dogBreed { get; set; }
        [BsonElement("dogSize")]
        public string dogSize { get; set; }
        //appointment details - booked
        [BsonElement("booked")]
        public bool booked { get; set; }
        [BsonElement("bookedStore")]
        public string bookedStore{ get; set; }
        [BsonElement("bookedAddress")]
        public string bookedAddress { get; set; }
        [BsonElement("bookedDate")]
        public DateTime bookedDate { get; set; }
        [BsonElement("bookedTime")]
        public DateTime bookedTime { get; set; }

        //appontment details not booked
        [BsonElement("prefStore")]
        public string prefStore { get; set; }
        [BsonElement("prefDate")]
        public DateTime prefDate { get; set; }
        [BsonElement("prefAddress")]
        public string prefAddress { get; set; }

        //payed
        [BsonElement("paid")]
        public bool paid { get; set; }
        [BsonElement("payer")]
        public string payer { get; set; }
        [BsonElement("receipt")]   //change once documents
        public string receipt { get; set; }

        //not payed
        [BsonElement("price")]
        public double price { get; set; }


        //services
        public class services
        {
            public bool wash { get; set; }
            public bool cut { get; set; }
            public bool trim { get; set; }
            public bool nailClean { get; set; }
            public bool earClean { get; set; }
            public bool teethClean { get; set; }

        }


        public List<services> Services
        {
            get;
            set;

        }

        [BsonElement("AdditionalInfo")]
        public string AdditionalInfo { get; set; }
    }
}