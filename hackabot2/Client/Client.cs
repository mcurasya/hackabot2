using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using hackabot.Commands;
using hackabot.Db.Model;
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
        public List < (Account, EitherStrict<ICommand, IEnumerable<IOneOfMany>>) > AccountCommandPair = new List < (Account, EitherStrict<ICommand, IEnumerable<IOneOfMany>>) > ();
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

            Bot = new TelegramBotClient(token);
            Bot.OnMessage += OnMessageRecieved;
            Bot.OnCallbackQuery += OnQueryReceived;
            Bot.StartReceiving();
        }

        private TelegramBotClient Bot { get; }
        protected Dictionary<Func<CallbackQuery, Account, bool>, Query> Queries { get; set; }

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
            var chatId = message.Chat.Id;
            Account account;
            EitherStrict<ICommand, IEnumerable<IOneOfMany>> commands;
            try
            {

                var acc = AccountCommandPair.First(t => t.Item1.ChatId == chatId);
                account = acc.Item1;
                commands = acc.Item2;
            }
            catch
            {
                if (TelegramController.Accounts.ContainsKey(chatId))
                {
                    account = TelegramController.Accounts[chatId];
                    commands = EitherStrict.Right<ICommand, IEnumerable<IOneOfMany>>(new List<IOneOfMany>());
                }
                else
                {
                    var contoller = new TelegramController();
                    contoller.Start();
                    account = contoller.FromMessage(message);
                    account.Controller = contoller;
                    commands = EitherStrict.Right<ICommand, IEnumerable<IOneOfMany>>(new List<IOneOfMany>());
                }
            }

            Console.WriteLine(
                //$"Command: {command}, status: {commands.ToString()}");
                $"Command: {commands}, status: {commands.ToString()}");

            var resp = Response.Eval(account, message, this, commands);
            if (resp.IsLeft)
                await SendTextMessageAsync(resp.Left);
            else
                foreach (var right in resp.Right)
                {
                    await SendTextMessageAsync(right);
                }
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