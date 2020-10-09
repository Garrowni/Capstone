using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TermProjectUI.Models
{
    public class MailAddressModel
    {
        public string From
        {
            get;
            set;
        }
        public string To
        {
            get;
            set;
        }
        public string Subject
        {
            get;
            set;
        }
        public string Body
        {
            get;
            set;
        }

    }
}