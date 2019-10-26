using System.Collections.Generic;
using hackabot.Commands;
using hackabot.Db.Model;
using Telegram.Bot.Types;

namespace hackabot.Queries
{
    public class CreateBoardQuery : Query
    {
        public override string Alias => "createboard";

        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            return new Response(new WaitForBoardNameCommand()).EditTextMessage(account, message.Message.MessageId, "Enter name for board:");
        }
    }
}