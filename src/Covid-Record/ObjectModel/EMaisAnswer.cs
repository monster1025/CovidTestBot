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
}