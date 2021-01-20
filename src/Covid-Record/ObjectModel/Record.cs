using System;
using System.Collections.Generic;
using System.Text;

namespace Covid_Record.ObjectModel
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class DoctorInfo
    {
        public string first_name { get; set; }
        public string second_name { get; set; }
        public string last_name { get; set; }
        public DateTime availability_date { get; set; }
        public string room { get; set; }
        public string code { get; set; }
        public string clinic { get; set; }
        public string availableResourceId { get; set; }
        public string complexResourceId { get; set; }
    }

    public class Auth
    {
        public string session_id { get; set; }
    }

    public class CreateRecord
    {
        public string patient_id { get; set; }
        public string availableResourceId { get; set; }
        public string complexResourceId { get; set; }
        public string date { get; set; }
        public DoctorInfo doctorInfo { get; set; }
        public string code { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string specialityCode { get; set; }
        public string specialityName { get; set; }
        public Auth auth { get; set; }
        public string ticket { get; set; }
    }
}
