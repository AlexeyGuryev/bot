using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
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
            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
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

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received choosen inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            InlineQueryResult[] results = {
                new InlineQueryResultLocation
                {
                    Id = "1",
                    Latitude = 40.7058316f, // displayed result
                    Longitude = -74.2581888f,
                    Title = "New York",
                    InputMessageContent = new InputLocationMessageContent // message if result is selected
                    {
                        Latitude = 40.7058316f,
                        Longitude = -74.2581888f,
                    }
                },

                new InlineQueryResultLocation
                {
                    Id = "2",
                    Longitude = 52.507629f, // displayed result
                    Latitude = 13.1449577f,
                    Title = "Berlin",
                    InputMessageContent = new InputLocationMessageContent // message if result is selected
                    {
                        Longitude = 52.507629f,
                        Latitude = 13.1449577f
                    }
                }
            };

            await Bot.AnswerInlineQueryAsync(inlineQueryEventArgs.InlineQuery.Id, results, isPersonal: true, cacheTime: 0);
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage) return;

            if (message.Text.StartsWith("/inline")) // send inline keyboard
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] // first row
                    {
                        new InlineKeyboardButton("1.1"),
                        new InlineKeyboardButton("1.2"),
                    },
                    new[] // second row
                    {
                        new InlineKeyboardButton("2.1"),
                        new InlineKeyboardButton("2.2"),
                    }
                });

                await Task.Delay(500); // simulate longer running task

                await Bot.SendTextMessageAsync(message.Chat.Id, "Choose",
                    replyMarkup: keyboard);
            }
            else if (message.Text.StartsWith("/keyboard")) // send custom keyboard
            {
                var keyboard = new ReplyKeyboardMarkup(new[]
                {
                    new [] // first row
                    {
                        new KeyboardButton("1.1"),
                        new KeyboardButton("1.2"),
                    },
                    new [] // last row
                    {
                        new KeyboardButton("2.1"),
                        new KeyboardButton("2.2"),
                    }
                });

                await Bot.SendTextMessageAsync(message.Chat.Id, "Choose",
                    replyMarkup: keyboard);
            }
            else if (message.Text.StartsWith("/photo")) // send a photo
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                const string file = @"Photo.jpg";

                var fileName = file.Split('\\').Last();

                using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var fts = new FileToSend(fileName, fileStream);

                    await Bot.SendPhotoAsync(message.Chat.Id, fts, "Nice Picture");
                }
            }
            else if (message.Text.StartsWith("/audio"))
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadAudio);

                const string file = @"ab.mp3";
                using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var fts = new FileToSend(file, fileStream);
                    await Bot.SendAudioAsync(message.Chat.Id, fts, 5, "you", "ab");
                }
            }
            else if (message.Text.StartsWith("/request")) // request location or contact
            {
                var keyboard = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton("Location")
                    {
                        RequestLocation = true
                    },
                    new KeyboardButton("Contact")
                    {
                        RequestContact = true
                    },
                });

                await Bot.SendTextMessageAsync(message.Chat.Id, "Who or Where are you?", replyMarkup: keyboard);
            }
            else if (message.Text.StartsWith("/"))
            {
                var usage = @"Usage:
                    type chords and press send, like: ccdb
                ";

                await Bot.SendTextMessageAsync(message.Chat.Id, usage,
                    replyMarkup: new ReplyKeyboardHide());
            }
            else
            {
                var outputFileName = string.Format("{0}.mp3", message.Text);
                var combiner = new AudioCombiner("chords");
                combiner.Combine(outputFileName, message.Text.ToCharArray().Select(l => string.Format("{0}.mp3", l)));

                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadAudio);

                using (var fileStream = new FileStream("chords/" + outputFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var fts = new FileToSend(outputFileName, fileStream);
                    await Bot.SendAudioAsync(message.Chat.Id, fts, 1 * message.Text.Length, "you", message.Text);
                }                
            }
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id,
                $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
        }
    }
}
