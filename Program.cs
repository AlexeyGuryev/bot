using System;
using System.IO;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("245637405:AAEsuVYs2RsulYb7eCeqKJFw0sGkxHl0wRk");

        public static void Main(string[] args)
        {            
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            var me = Bot.GetMeAsync().Result;

            Console.Title = me.Username;

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Something wrong in Telegram:");
            Console.WriteLine(receiveErrorEventArgs.ApiRequestException);
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage) return;

            var parser = new ChordParser(message.Text);
            if (parser.GetChordNames().Any())
            {
                var outputFileName = string.Format("{0}.mp3", message.Text);
                var combiner = new AudioCombiner("chords");

                combiner.Combine(outputFileName,
                    parser.GetChordNames().Select(l => string.Format("{0}.mp3", l)));

                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadAudio);

                using (
                    var fileStream = new FileStream("chords/" + outputFileName, FileMode.Open, FileAccess.Read,
                        FileShare.Read))
                {
                    var fts = new FileToSend(outputFileName, fileStream);
                    await Bot.SendAudioAsync(message.Chat.Id, fts, 0, "Great Author", message.Text);
                }
            }
            else
            {
                var usage = @"Usage: Just type chords and press send, like: CmCDDm or cmcddm or cm c d Dm";

                await Bot.SendTextMessageAsync(message.Chat.Id, usage,
                    replyMarkup: new ReplyKeyboardHide());
            }
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id,
                $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
        }
    }
}
