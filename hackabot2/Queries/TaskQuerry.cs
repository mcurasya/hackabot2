using System.Collections.Generic;
using hackabot.Db.Model;
using Telegram.Bot.Types;

namespace hackabot.Queries
{
    public class TaskQuerry : Query
    {
        public override string Alias { get; } = "get_task";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            throw new System.NotImplementedException();
        }
    }
}