using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotFramework.Commands;
using BotFramework.Queries;
using hackabot;
using Microsoft.EntityFrameworkCore;
using Monad;
using StickerMemeDb.Controllers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace BotFramework.Client
{
    public partial class Client
    {
        public List<(Account, EitherStrict<ICommand, IEnumerable<IOneOfMany>>)> AccountCommandPair;
        public Client(string token, Assembly assembly)
        {
            var baseType = typeof(Query);
            assembly = baseType.Assembly;

            Queries = assembly
                      .GetTypes()
                      .Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract)
                      .Select(c => Activator.CreateInstance(c) as Query)
                      .Where(c => c != null)
                      .ToDictionary(x => new Func<CallbackQuery, Account, bool>(x.IsSuitable), x => x);

            Bot                 =  new TelegramBotClient(token);
            Bot.OnMessage       += OnMessageRecieved;
            Bot.OnCallbackQuery += OnQueryReceived;
            Bot.StartReceiving();
        }

        private   TelegramBotClient                                     Bot      { get; }
        protected Dictionary<Func<CallbackQuery, Account, bool>, Query> Queries  { get; set; }

        //public async void OnUpdateReceived(object sender, UpdateEventArgs e)
        //{
        //    if (e.Update.Type == UpdateType.ChannelPost)
        //    {
        //        var message    = e.Update.ChannelPost;
        //        var controller = new TelegramController();
        //        controller.Start();
        //        if (message.Document != null || message.Photo != null)
        //        {
        //            var meme = await ImageDownloader.DownloadFromMessage(message, this,
        //                           new ChatId() {ChatId = message.Chat.Id, UserName = message.Chat.Username, Id = -1},
        //                           controller);
        //            await EditMessageReplyMarkupAsync(message.Chat, message.MessageId,
        //                Command.AddMemeButton(controller.GetChannelLanguage(message.Chat.Id), controller, meme.Id,
        //                    controller.CountLikes(meme.Id)));
        //        }
        //    }
        //}

        public async void HandleQuery(CallbackQuery query)
        {
            try
            {
                var contoller = new TelegramController();
                contoller.Start();

                var account = contoller.FromQuery(query);

                if (account == null)
                {
                    await Bot.AnswerCallbackQueryAsync(query.Id, "Your account doesn't exist.");
                    return;
                }

                account.Controller = account.Controller ?? contoller;
                var command = GetQuery(query, account);

                Console.WriteLine($"Command: {command}");

                await SendTextMessageAsync(command.Execute(query, account));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async void HandleMessage(Message message)
        {
            var     chatId = message.Chat.Id;
            Account account;

            if (AccountCommandPair.Find(t => t.Item1.Chat))
            if (TelegramController.Accounts.ContainsKey(chatId))
            {
                account = TelegramController.Accounts[chatId];
            }
            else
            {
                var contoller = new TelegramController();
                contoller.Start();
                account            = contoller.FromMessage(message);
                account.Controller = contoller;
            }

            var command = GetCommand(message, account);

            Console.WriteLine(
                $"Command: {command}, status: {account.Status.ToString()}");

            await SendTextMessageAsync(command.Execute(message, this, account));
        }


        protected Query GetQuery(CallbackQuery message, Account account)
        {
            var func = Queries.Keys.FirstOrDefault(s => s.Invoke(message, account));
            return func != null ? Queries[func] : default;
        }

        public void OnMessageRecieved(object sender, MessageEventArgs e)
        {
            Console.WriteLine(DateTime.Now.ToShortTimeString() + " " + e.Message.From.Username + ": " + e.Message.Text);
            try
            {
                HandleMessage(e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void OnQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            Console.WriteLine(
                $"{DateTime.Now.ToShortTimeString()} {e.CallbackQuery.From.Username}: {e.CallbackQuery.Data}");
            try
            {
                HandleQuery(e.CallbackQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}