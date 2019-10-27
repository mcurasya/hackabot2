using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using hackabot.Commands;
using hackabot.Db.Model;
using hackabot.Notifications;
using hackabot.Queries;
using hackabot2.Db.Controllers;
using Monad;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace hackabot.Client
{
    public partial class Client
    {
        public NotificationManager NotificationManager { get; set; }

        public Client(string token)
        {
            var baseType = typeof(Command);
            var assembly = baseType.Assembly;

            Commands = assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract)
                .Select(c => Activator.CreateInstance(c) as Command)
                .Where(c => c != null)
                .ToDictionary(x => new Func<Message, Account, bool>(x.Suitable), x => x);

            baseType = typeof(Query);
            Queries = assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract)
                .Select(c => Activator.CreateInstance(c) as Query)
                .Where(c => c != null)
                .ToDictionary(x => new Func<CallbackQuery, Account, bool>(x.IsSuitable), x => x);

            Bot = new TelegramBotClient(token);
            NotificationManager = new NotificationManager(this);
            //NotificationManager.Cycle();
            Bot.OnMessage += OnMessageRecieved;
            Bot.OnCallbackQuery += OnQueryReceived;
            Bot.DeleteWebhookAsync();
            Bot.StartReceiving();
        }

        private TelegramBotClient Bot { get; }

        protected Dictionary<Func<Message, Account, bool>, Command> Commands { get; set; }
        protected Dictionary<Func<CallbackQuery, Account, bool>, Query> Queries { get; set; }

        public async void HandleQuery(CallbackQuery query)
        {
            try
            {
                var controller = new TelegramController();
                controller.Start();

                var account = controller.FromQuery(query);

                if (account == null)
                {
                    await Bot.AnswerCallbackQueryAsync(query.Id, "Your account doesn't exist.");
                    return;
                }

                account.Controller = account.Controller ?? controller;
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
            var chatId = message.From.Id;
            Account account;

            if (TelegramController.Accounts.ContainsKey(chatId))
            {
                account = TelegramController.Accounts[chatId];
            }
            else
            {
                var contoller = new TelegramController();
                contoller.Start();
                account = contoller.FromMessage(message);
                account.Controller = contoller;
            }

            var command = GetCommand(message, account);

            Console.WriteLine(
                $"Command: {command}, status: {account.Status.ToString()}");

            await SendTextMessageAsync(command.Execute(message, this, account));
        }
        protected Command GetCommand(Message message, Account account)
        {
            var key = Commands.Keys.FirstOrDefault(s => s.Invoke(message, account));
            if (key == null) return new StartCommand();
            return Commands[key];
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