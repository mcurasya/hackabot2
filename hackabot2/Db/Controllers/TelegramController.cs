using System;
using System.Collections.Generic;
using System.Linq;
using hackabot;
using hackabot.Db.Model;
using Telegram.Bot.Types;

namespace hackabot2.Db.Controllers

{
    public class TelegramController
    {
        private static bool Delete;
        private static bool First = true;
        public TelegramContext Context;

        public void Start()
        {
            Context = new TelegramContext();
            Context.Database.EnsureCreated();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        #region ChatId

        public static Dictionary<long, Account> Accounts = new Dictionary<long, Account>();

        public Account FromId(int id)
        {
            var account = Accounts.Values.FirstOrDefault(a => a.Id == id);
            if (account != null)
            {
                return account;
            }

            account = Context.Accounts.Find(id);
            Accounts.Add(account.ChatId, account);

            return account;
        }

        public Account FromMessage(Message message)
        {

            if (Accounts.ContainsKey(message.Chat.Id))
                return Accounts[message.Chat.Id];
            var account = Context.Accounts.FirstOrDefault(a => a.ChatId == message.Chat.Id);

            if (account == null)
            {
                account = new Account();
                account.ChatId = message.Chat.Id;
                account.Controller = this;
                account.Username = message.Chat.Username;
            }
            if (!Accounts.ContainsKey(account.ChatId))
                Accounts.Add(account.ChatId, account);

            return account;
        }

        public Account FromQuery(CallbackQuery message)
        {
            var account = Context.Accounts.FirstOrDefault(a => a.ChatId == message.From.Id);
            if (account != null) return account;
            //TODO create new  account maybe? or do something idk  
            account = new Account
            {
                ChatId = message.From.Id
            };
            Context.Accounts.Add(account);
            SaveChanges();

            return account;
        }

        #endregion

        #region Boards

        public Board GetBoard(int id) => Context.Boards.Find(id);

        #endregion

        #region DBQueries

        public void AddTask(Task task)
        {

        }

        public void AddBoard(Board board)
        {

        }

        public void AssignWorkerToBoard(WorkerToBoard value)
        {

        }

        public void ChangePriority(Task task, Priorities priorities)
        {

        }

        public void ChangeWorkerAccessLevel(WorkerToBoard worker, Board board, AccessLevel accessLevel)
        {

        }

        #endregion
    }
}