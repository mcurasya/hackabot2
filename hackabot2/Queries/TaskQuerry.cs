using System;
using System.Collections.Generic;
using System.Linq;
using hackabot.Db.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace hackabot.Queries
{
    public class TaskQuerry : Query
    {
        public override string Alias { get; } = "get_task";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var board = account.Controller.GetBoards(account).First(b => b.Id.ToString() == values.First().Key);
            var taskId = values.First().Value;
            var task = account.Controller.GetTasks(board, account);
            return null;
        }
        public static InlineKeyboardMarkup TaskButton(Task task)
        {
            throw new NotImplementedException();
        }
    }
}