using System;
using System.Collections.Generic;
using System.Text;

namespace Covid_Record.ObjectModel
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class BaseResponse
    {
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public object result { get; set; }
        public double execTime { get; set; }
        public string request_id { get; set; }
        public string session_id { get; set; }
    }

}
