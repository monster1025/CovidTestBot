using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Covid_Record.ObjectModel;
using Covid_Record.ObjectModel.recordInfo;
using Newtonsoft.Json;
using DoctorInfo = Covid_Record.ObjectModel.doctorinfo.DoctorInfo;

namespace Covid_Record
{
    public class CovidAvailability
    {
        //2009 - Антитела
        //2010 - ПЦР
        //2011 - Вакцинация
        //2004 - Пост - для теста
        const string specialityCode = "2009"; //"2009";
        private readonly TelegramBot _telegram;
        private Record _currentRecord;

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

        public void GetCurrentAppointments(int patientId, string session)
        {
            _currentRecord = GetAppointments(patientId, session, specialityCode)?.FirstOrDefault();
        }

        public bool IsAvailable(string session, int patientId)
        {
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

                var enableAppointmentAutoCreation = true;
                var nearestDate = responseJson.result.clinics.FirstOrDefault()?.doctors_info?.FirstOrDefault();
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (enableAppointmentAutoCreation && nearestDate != null)
                {
                    var createAppointment = new CreateRecord()
                    {
                        availableResourceId = nearestDate.availableResourceId,
                        complexResourceId = nearestDate.complexResourceId,
                        auth = new Auth {session_id = session},
                        code = nearestDate.code,
                        date = DateTime.Parse(nearestDate.availability_date).Date.ToString("dd.MM.yyyy"),
                        doctorInfo = new ObjectModel.DoctorInfo
                        {
                            availability_date = DateTime.Parse(nearestDate.availability_date),
                            code = nearestDate.code,
                            availableResourceId = nearestDate.availableResourceId,
                            complexResourceId = nearestDate.complexResourceId,
                            clinic = nearestDate.clinic,
                            first_name = nearestDate.first_name,
                            last_name = nearestDate.last_name,
                            room = nearestDate.room,
                            second_name = nearestDate.second_name
                        },
                        patient_id = patientId.ToString(),
                        specialityCode = specialityCode,
                        specialityName = "",
                        startTime = DateTime.Parse(nearestDate.availability_date).ToString("dd.MM.yyyy HH:mm"),
                        endTime = DateTime.Parse(nearestDate.availability_date).AddMinutes(5).ToString("dd.MM.yyyy HH:mm"),
                        ticket = _currentRecord?.ticket
                    };

                    string responseCreateAppointmentJson;
                    if (!string.IsNullOrEmpty(createAppointment.ticket))
                    {
                        if (DateTime.Parse(_currentRecord?.dateBegin ?? "") > DateTime.Parse(nearestDate.availability_date))
                        {
                            responseCreateAppointmentJson = Post(
                                "https://emp.mos.ru/v1.1/doctor/shiftAppointment?token=887033d0649e62a84f80433e823526a1",
                                JsonConvert.SerializeObject(createAppointment));
                            GetCurrentAppointments(patientId, session);
                        }
                    }
                    else
                    {
                        responseCreateAppointmentJson = Post(
                            "https://emp.mos.ru/v1.1/doctor/createAppointment?token=887033d0649e62a84f80433e823526a1",
                            JsonConvert.SerializeObject(createAppointment));
                        GetCurrentAppointments(patientId, session);
                    }
                }

                Console.WriteLine(sb.ToString());
                _telegram.SendMessage(sb.ToString());
            }

            return true;
        }

        public List<Record> GetAppointments(int patientId, string session, string specialityCode = null)
        {
            var json =
                "{ \"patient_id\":\"" + patientId + "\",\"auth\":{ \"session_id\":\"" + session + "\"} }";

            var response = Post("https://emp.mos.ru/v1.1/doctor/getList?token=887033d0649e62a84f80433e823526a1", json);
            var responseDecoded = JsonConvert.DeserializeObject<RecordListResponse>(response);

            var records = responseDecoded.result.records;
            if (!string.IsNullOrEmpty(specialityCode))
            {
                records = records.Where(f => f.doctor.speciality_code == specialityCode).ToList();
            }
            return records;
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
