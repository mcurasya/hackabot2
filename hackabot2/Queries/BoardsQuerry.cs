using System.Collections.Generic;
using hackabot.Db.Model;
using Telegram.Bot.Types;

namespace hackabot.Queries
{
    public class BoardsQuerry : Query
    {
        public override string Alias { get; } = "Boards";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            account.Controller.GetBoards();
            return new Response(account, "Here is your boards", account.);
        }
    }
}