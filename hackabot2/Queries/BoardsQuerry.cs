using System.Collections.Generic;
using System.Linq;
using hackabot.Db.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot.Queries
{
    public class BoardsQuerry : Query
    {
        public override string Alias { get; } = "Boards";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var boards = account.Controller.GetBoards(account);
            return new QuerryResponse().EditMessageMarkup(account, message.Message.MessageId, BoardsKeyboard(boards));
        }
        //todo askold
        public static InlineKeyboardMarkup BoardsKeyboard(IEnumerable<Board> boards) => new InlineKeyboardMarkup(boards.Select(t => t.Name));
    }
}