using System;
using System.Threading;

namespace Covid_Record
{
    class Program
    {
        static void Main(string[] args)
        {
            var telegram = new TelegramBot();
            var arr = new CovidAvailability(telegram);

            var phone = "7916xxxxx";
            var password = "xxxxx";
            int? patientId = null;

            var session = arr.GetSession(phone, password);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (patientId == null)
            {
                patientId = arr.GetPatient(session);
            }

            if (patientId == null)
            {
                Console.WriteLine("Не могу получить ID пациента.");
                Console.ReadLine();
                return;
            }

            arr.UpdateAppointments(patientId.Value, session);
            while (true)
            {
                arr.IsAvailable(session, patientId.Value);
                Thread.Sleep(TimeSpan.FromSeconds(4));
            }
        }
    }
}
