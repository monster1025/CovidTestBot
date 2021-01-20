using System;
using System.Collections.Generic;
using System.Text;

namespace Covid_Record.ObjectModel.recordInfo
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Doctor
    {
        public string doctor_specialty { get; set; }
        public string speciality_code { get; set; }
        public string doctor_fio { get; set; }
        public string code { get; set; }
    }

    public class Record
    {
        public string ticket { get; set; }
        public string clinic { get; set; }
        public string dateBegin { get; set; }
        public string visit_time { get; set; }
        public Doctor doctor { get; set; }
    }

    public class Result
    {
        public string oms_number { get; set; }
        public string fio { get; set; }
        public List<Record> records { get; set; }
    }

    public class RecordListResponse
    {
        public int errorCode { get; set; }
        public Result result { get; set; }
        public string errorMessage { get; set; }
        public double execTime { get; set; }
        public string request_id { get; set; }
        public string session_id { get; set; }
    }


}
