using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;

namespace hackabot.Controllers

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
            var start = message.Text?.Length > "/start".Length && message.Text.StartsWith("/start");
            if (Accounts.ContainsKey(message.Chat.Id) && !start) return Accounts[message.Chat.Id];
            var account = Context.Accounts.FirstOrDefault(a => a.ChatId == message.Chat.Id);
            if (message.Text != null)
                if (start)
                {
                    var param = message.Text.Substring(7);
                    var base64EncodedBytes = Convert.FromBase64String(param);
                    param = Encoding.UTF8.GetString(base64EncodedBytes);
                    var p = param.Split('*');

                    //todo start command
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

    }
}