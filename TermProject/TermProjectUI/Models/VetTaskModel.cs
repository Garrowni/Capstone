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

        [BsonElement("requester")]
        public string requester { get; set; }
        [BsonElement("reqPhoto")]
        public string reqPhoto { get; set; }


        [BsonElement("posterPhoto")]
        public string posterPhoto { get; set; }


        [BsonElement("ImportanceLevel")]
        public int ImportanceLevel { get; set; }
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
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Enter a Comment")]
        public string singleComm { get; set; }
        public class Comment
        {

            public string commId { get; set; }
            public string volunteerId { get; set; }
            public string volunteerPhoto { get; set; }
            public string volunteerName { get; set; }
            public string comm { get; set; }
        }

        public List<Comment> Comments
        {
            get;
            set;

        }



        //pickup
        [BsonElement("pickupLocation")]
        public string pickupLocation { get; set; }
        [BsonElement("pickupVolunteer")]
        public string pickupVolunteer { get; set; }
        [BsonElement("PUDate"), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true), DataType(DataType.Date, ErrorMessage = "Date only")]
        public DateTime PUDate { get; set; }
        [BsonElement("PUTime")]
        public TimeSpan PUTime { get; set; }

        //appointment
        [BsonElement("appointmentAddress")]
        public string appointmentAddress { get; set; }
        [BsonElement("APDate"), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true), DataType(DataType.Date, ErrorMessage = "Date only")]
        public DateTime APDate { get; set; }
        [BsonElement("APTime")]
        public TimeSpan APTime { get; set; }


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
        [BsonElement("DODate"), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true), DataType(DataType.Date, ErrorMessage = "Date only")]
        public DateTime DODate { get; set; }
        [BsonElement("DOTime")]
        public TimeSpan DOTime { get; set; }

        //dog
        [BsonElement("dogName")]
        public string dogName { get; set; }
        [BsonElement("dogBreed")]
        public string dogBreed { get; set; }
        [BsonElement("dogSize")]
        public string dogSize { get; set; }
        [BsonElement("dogNotes")]
        public string dogNotes { get; set; }



        [BsonElement("AdditionalInfo")]
        public string AdditionalInfo { get; set; }
        //documents

        public string File { get; set; }

        public List<string> FileList { get; set; }


 

    }
}