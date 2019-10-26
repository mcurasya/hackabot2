using System.Collections.Generic;
using System.Linq;
using hackabot.Db.Model;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot
{
    public static class Keyboards
    {
        public static ReplyKeyboardMarkup Main(Account account)
        {
            return new ReplyKeyboardMarkup(new KeyboardButton[]
            {
                "Boards",
            });
        }
        public static InlineKeyboardMarkup CreateBoards(Account account)
        {
            return new InlineKeyboardButton[]
            {
                new InlineKeyboardButton
                {
                    Text = "Create Board",
                        CallbackData = "createboard"

                },

            };
        }
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
                        CallbackData = Queries.Query.PackParams("get_board", "id", board.Id.ToString())
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