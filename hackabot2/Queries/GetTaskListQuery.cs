using System.Collections.Generic;
using System.Linq;
using hackabot.Db.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot.Queries
{
    public class GetTaskListQuery : Query
    {
        public override string Alias { get; } = "get_task_list";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            //aaah i dont know why it does not work
            int boardId = int.Parse(values["id"]);
            var board = account.Controller.GetBoard(boardId);
            return new Response().EditMessageMarkup(account.ChatId, message.Message.MessageId, BoardButton(board));
        }
        //this is with taks, not boards
        public static InlineKeyboardMarkup BoardButton(Board board)
        {

            var input = board.Tasks?.ToArray();
            var keys = new List<List<InlineKeyboardButton>>();

            for (int i = 0; i < input?.Length; i++)
            {
                var task = input[i];
                var button =
                    new InlineKeyboardButton()
                    {
                        Text = task.Name,
                        CallbackData = PackParams("edit_task", board.Id.ToString(), task.Id.ToString())
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
            keys.Add(new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton
                {
                    Text = "<<",
                        CallbackData = Queries.Query.PackParams("get_board", "id", board.Id.ToString())
                }
            });
            return keys.ToArray();
        }
    }
}