using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;

namespace Covid_Record
{
    public class TelegramBot
    {
        private TelegramBotClient _telegram;
        public string Token => "1479535614:AAFqHdsUFikzOti0zknK_pN2sEONNAXecPY";
        public long MainChatId;

        public TelegramBot(long chatId = -415825856)
        {
            MainChatId = chatId;
            _telegram = new TelegramBotClient(Token);
            _telegram.OnMessage += Bot_OnMessage;
            _telegram.StartReceiving();
        }

        public void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            Console.WriteLine($"[{e.Message.Chat.Id}, user: {e.Message.From.Id}] @{e.Message.From?.Username}: {e.Message.Text}");
        }

        public void SendMessage(string message, long? chatId = null)
        {
            _telegram.SendTextMessageAsync(chatId ?? MainChatId, message).GetAwaiter().GetResult();
        }
    }
}
