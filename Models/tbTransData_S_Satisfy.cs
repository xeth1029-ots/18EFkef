using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WKEFSERVICE.Models
{
    public class tbTransData_S_Satisfy : Entities.TransData_S_Satisfy
    {
        public string ClassDateS { get; set; }
        public string ClassDateE { get; set; }
        public string TrainingTimeS { get; set; }

    }
}