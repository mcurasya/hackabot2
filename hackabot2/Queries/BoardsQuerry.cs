using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public static InlineKeyboardMarkup BoardsKeyboard(IEnumerable<Board> boards)
        {
            var input = boards.ToArray();
            var keys = new List<List<InlineKeyboardButton>>();
            
            for (int i = 0; i < input.Length; i++)
            {
                var board = input[i];
                var button =
                    new InlineKeyboardButton()
                    {
                        Text = board.Name,
                        CallbackData = PackParams("get_board", board.Name, board.Name)
                    };
                if (keys.Count == 0)
                {
                    keys.Add(new List<InlineKeyboardButton> { button });
                }
                else if (keys.Count > 0)
                {
                    if (keys.Last().Count == 1)
                    {
                        keys.Last().Add(button);
                    }
                    else
                    {
                        keys.Add(new List<InlineKeyboardButton> { button });
                    }
                }
            }
            return keys.ToArray();
        }
    }
}