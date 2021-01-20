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

            var phone = "7xxxxxxx";
            var password = "xxxx";

            var session = arr.GetSession(phone, password);

            var patient = arr.GetPatient(session);
            if (patient == null)
            {
                Console.WriteLine("Не могу получить ID пациента.");
                Console.ReadLine();
                return;
            }

            arr.GetCurrentAppointments(patient ?? -1, session);

            while (true)
            {
                arr.IsAvailable(session, patient.Value);
                Thread.Sleep(TimeSpan.FromSeconds(4));
            }
        }
    }
}
