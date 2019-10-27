using System.Collections.Generic;
using hackabot.Db.Model;
using Telegram.Bot.Types;

namespace hackabot.Queries
{
    public class BoardListQuery : Query
    {
        public override string Alias => "board_list";

        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var boards = account.Controller.GetBoards(account);

            return new Response().EditTextMessage(account, message.Message.MessageId, $"You have {boards.Length} boards.\nChoose one board to get more details:",
                Keyboards.BoardsKeyboard(boards));
        }
    }
}