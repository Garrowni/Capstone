using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TermProjectUI.Models
{
    public class LogInModel
    {
        [BsonElement("Email")]
        [Required(ErrorMessage ="Email is Required")]
        public string Email { get; set; }
        [BsonElement("Password"),DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is Required")]

        public string Password { get; set; }
       
    }
}