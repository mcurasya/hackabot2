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
    }
}