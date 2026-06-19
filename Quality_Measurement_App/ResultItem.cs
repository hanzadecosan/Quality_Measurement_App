using System;
using System.Collections.Generic;
using System.Text;

namespace Quality_Measurement_App
{
    public class ResultItem
    {
        public int StepNo { get; set; }
        public int CriteriaID { get; set; }
        public string CriteriaName { get; set; }
        public string EnteredValue { get; set; }
        public string Status { get; set; }

        public decimal? NumericValue { get; set; }
        public string TextValue { get; set; }

        public int StatusID { get; set; }
    }
}
