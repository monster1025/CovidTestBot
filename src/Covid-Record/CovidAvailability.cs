using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Covid_Record
{
    public class CovidAvailability
    {
        private readonly TelegramBot _telegram;

        public CovidAvailability(TelegramBot telegram)
        {
            _telegram = telegram;
        }

        public string GetSession(string phone, string password)
        {
            var content = "{\"auth\":{\"login\":\"" + phone + "\",\"password\":\"" + password +"\",\"guid\":\"" + Guid.NewGuid() + "\"}}";
            var sessionResponse = Post("https://emp.mos.ru/v1.1/auth/virtualLogin?token=887033d0649e62a84f80433e823526a1", content);
            dynamic data = JsonConvert.DeserializeObject(sessionResponse);
            var sessionId = (data.session_id.Value as string);
            return sessionId;
        }

        public int? GetPatient(string session)
        {
            var content = "{\"auth\":{\"session_id\":\"" + session + "\"}}";
            var patientResponse = Post("https://emp.mos.ru/v1.1/patient/get?token=887033d0649e62a84f80433e823526a1", content);
            var data = JsonConvert.DeserializeObject<PatientResponse>(patientResponse);
            return data?.result?.FirstOrDefault()?.id;
        }

        public bool IsAvailable(string session, int patientId)
        {
            //2010 - covid
            //2004 - Пост - для теста
            var specialityCode = "2010";
            var json =
                "{ \"patient_id\":\"" + patientId + "\",\"speciality_code\":\"" + specialityCode + "\",\"auth\":{ \"session_id\":\"" + session + "\"} }";

            var response = Post("https://emp.mos.ru/v1.1/doctor/getDoctors?token=887033d0649e62a84f80433e823526a1",
                json);

            var responseJson = JsonConvert.DeserializeObject<EMaisAnswer>(response);

            Console.WriteLine($"[{DateTime.Now}] {responseJson.errorCode}: {responseJson.errorMessage}");

            if (responseJson.errorCode == 0)
            {
                var sb = new StringBuilder();

                foreach (var clinic in responseJson.result.clinics)
                {
                    sb.AppendLine($"Клиника: {clinic.address}");
                    foreach (var doctorsInfo in clinic.doctors_info)
                    {
                        sb.AppendLine($"- {doctorsInfo.availability_date}: {doctorsInfo.last_name} {doctorsInfo.first_name} {doctorsInfo.second_name}");
                    }
                }

                Console.WriteLine(sb.ToString());
                _telegram.SendMessage(sb.ToString());
            }

            return true;
        }

        public string Post(string url, string content, string contentType = "application/json")
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.ContentType = contentType;
            httpRequest.Method = "POST";
            httpRequest.CookieContainer = new CookieContainer();
            httpRequest.Headers.Add("User-Agent", "okhttp/4.2.2");

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(content);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }
    }
}
