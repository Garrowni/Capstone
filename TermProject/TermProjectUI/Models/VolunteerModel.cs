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
    public class VolunteerModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Name")]
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }

        [BsonElement("Email")]
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [BsonElement("Password"), DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }
        [BsonElement("ConfirmPassword"), DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        [Required(ErrorMessage = "Confirm Password is Required")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "UserPhoto")]
        
        public string UserPhoto { get; set; }

        [BsonElement("Role")]
        
        public string Role { get; set; }
        [Display(Name = "Active")]

        public string Active { get; set; }
    }
}