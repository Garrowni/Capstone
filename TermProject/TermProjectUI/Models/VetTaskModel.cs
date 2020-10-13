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
    public class VetTaskModel
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



        //pickup
        [BsonElement("pickupLocation")]
        public string pickupLocation { get; set; }
        [BsonElement("pickupVolunteer")]
        public string pickupVolunteer { get; set; }
        [BsonElement("pickupDate")]
        public DateTime pickupDate { get; set; }
        [BsonElement("pickupTime")]
        public DateTime pickupTime { get; set; }

        //appointment
        [BsonElement("appointmentAddress")]
        public string appointmentAddress { get; set; }
        [BsonElement("appointmentDate")]
        public DateTime appointmentDate { get; set; }
        [BsonElement("appointmentTime")]
        public DateTime appointmentTime{ get; set; }
        [BsonElement("vetName")]
        public string vetName { get; set; }
        [BsonElement("appointmentReason")]
        public string appointmentReason { get; set; }
        [BsonElement("appointmentNotes")]
        public string appointmentNotes { get; set; }


        //dropoff
        [BsonElement("dropoffLocation")]
        public string dropoffLocation { get; set; }
        [BsonElement("dropoffVolunteer")]
        public string dropoffVolunteer { get; set; }
        [BsonElement("dropoffDate")]
        public DateTime dropoffDate { get; set; }
        [BsonElement("dropoffTime")]
        public DateTime dropoffTime { get; set; }



        //dog
        [BsonElement("dogName")]
        public string dogName { get; set; }
        [BsonElement("dogBreed")]
        public string dogBreed { get; set; }
        [BsonElement("dogSize")]
        public string dogSize { get; set; }
        [BsonElement("dogNotes")]
        public string dogNotes { get; set; }

        //documents


        public class documents
        {
            public string Document { get; set; }  //not sure how to do this yet
            public string Descrition { get; set; }

        }




        public List<documents> Documents
        {
            get;
            set;

        }




        [BsonElement("AdditionalInfo")]
        public string AdditionalInfo { get; set; }

    }
}