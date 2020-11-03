using System.Collections.Generic;

namespace Covid_Record
{
    public class EMaisAnswer
    {
        public int errorCode { get; set; }
        public Result result { get; set; }
        public string errorMessage { get; set; }
        public double execTime { get; set; }
        public string request_id { get; set; }
        public string session_id { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class DoctorsInfo
    {
        public string availableResourceId { get; set; }
        public string complexResourceId { get; set; }
        public string first_name { get; set; }
        public string second_name { get; set; }
        public string last_name { get; set; }
        public string availability_date { get; set; }
        public string room { get; set; }
        public string clinic { get; set; }
        public string code { get; set; }
    }

    public class Clinic
    {
        public string clinic { get; set; }
        public string address { get; set; }
        public List<DoctorsInfo> doctors_info { get; set; }
    }

    public class Result
    {
        public List<Clinic> clinics { get; set; }
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Patient
    {
        public string oms_number { get; set; }
        public string birthdate { get; set; }
        public int id { get; set; }
        public string lastname { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
    }

    public class PatientResponse
    {
        public int errorCode { get; set; }
        public List<Patient> result { get; set; }
        public string errorMessage { get; set; }
        public double execTime { get; set; }
        public string request_id { get; set; }
        public string session_id { get; set; }
    }


}