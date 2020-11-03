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

            var phone = "79160000000";
            var password = "testpwd";

            var session = arr.GetSession(phone, password);

            var patient = arr.GetPatient(session);
            if (patient == null)
            {
                Console.WriteLine("Не могу получить ID пациента.");
                Console.ReadLine();
                return;
            }

            while (true)
            {
                arr.IsAvailable(session, patient);
                Thread.Sleep(TimeSpan.FromSeconds(4));
            }
        }
    }
}
