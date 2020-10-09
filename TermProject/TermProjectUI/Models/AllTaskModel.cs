using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TermProjectUI.Models
{
    public class AllTaskModel
    {
        
       
        public List<TransportationTaskModel> TransportationTasks
        {
            get;
            set;

        }
        public List<OtherTaskModel> OtherTasks
        {
            get;
            set;

        }

        public List<InventoryTaskModel> InventoryTasks
        {
            get;
            set;
        }


        public List<GroomingTaskModel> GroomingTasks
        {
            get;
            set;
        }

        public List<PhotographyTaskModel> PhotographyTasks
        {
            get;
            set;
        }

        public List<VetTaskModel> VetTasks
        {
            get;
            set;
        }
    }
}