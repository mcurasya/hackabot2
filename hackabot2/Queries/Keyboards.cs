using System.Collections.Generic;
using System.Linq;
using hackabot.Db.Model;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot.Queries
{
    public static class Keyboards
    {

        public static ReplyKeyboardMarkup BoardsKeyboard(IEnumerable<Board> boards) => null;
    }
}