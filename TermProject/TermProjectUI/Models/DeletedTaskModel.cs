using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TermProjectUI.Models
{
    public class DeletedTaskModel
    {
        public List<TransportationTaskModel> deletedTransTask
        {
            get;
            set;

        }
        public List<OtherTaskModel> deletedOtherTask
        {
            get;
            set;

        }
    }
}