using System.Collections.Generic;
using System.Linq;
using hackabot.Db.Model;
using Telegram.Bot.Types;

namespace hackabot.Queries
{
    public class CreateTaskQuery : Query
    {
        public override string Alias { get; } = "create_task";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var board = account.Controller.GetBoards(account).FirstOrDefault(t => t.Id.ToString() == values.First().Value);
            account.Status = AccountStatus.WaitForTaskName;
            return new Response().EditTextMessage(account.ChatId, message.Message.MessageId, "Please enter task name");
        }
    }
}