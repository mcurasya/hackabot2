using System.Collections.Generic;
using hackabot.Db.Model;
using Telegram.Bot.Types;

namespace hackabot.Queries
{
    public class GetBoardQuerry : Query
    {
        public override string Alias { get; } = "get_board";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            
        }
    }
}