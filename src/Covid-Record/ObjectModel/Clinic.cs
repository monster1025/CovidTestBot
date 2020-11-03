using System.Collections.Generic;

namespace Covid_Record
{
    public class Clinic
    {
        public string clinic { get; set; }
        public string address { get; set; }
        public List<DoctorsInfo> doctors_info { get; set; }
    }
}