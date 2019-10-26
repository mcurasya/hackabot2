using System;
using System.Collections.Generic;
using System.Linq;
using hackabot.Commands;
using hackabot.Db.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot.Queries
{
    public class GetBoardQuerry : Query
    {
        public override string Alias { get; } = "get_board";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var boardName = values.First().Value;
            try
            {
                var board = account.Controller.GetBoards(account).First(t => t.Name == boardName);
                return new QuerryResponse().EditMessageMarkup(account, message.Message.MessageId, BoardButton(board));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new BadInputException(new QuerryResponse().TextMessage(account.ChatId, "No such task"));
            }
        }

        public static InlineKeyboardMarkup BoardButton(Board board) => null;
        //new InlineKeyboardMarkup(new InlineKeyboardButton[]{});
    }
}