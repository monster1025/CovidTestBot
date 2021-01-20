using System;
using System.Collections.Generic;
using System.Text;

namespace Covid_Record.ObjectModel.doctorinfo
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class TimePeriod
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
    }

    public class Shedule
    {
        public string date { get; set; }
        public List<TimePeriod> time_periods { get; set; }
    }

    public class Result
    {
        public List<Shedule> shedules { get; set; }
    }

    public class DoctorInfo
    {
        public int errorCode { get; set; }
        public Result result { get; set; }
        public string errorMessage { get; set; }
        public double execTime { get; set; }
        public string request_id { get; set; }
        public string session_id { get; set; }
    }


}
