using System.Collections.Generic;
using System.Linq;
using hackabot.Db.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot.Queries
{
    public class TaskQuerry : Query
    {
        public override string Alias { get; } = "get_task";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var board = account.Controller.GetBoards(account).First(b => b.Id.ToString() == values.First().Key);
            var taskId = values.First().Value;
            var task = account.Controller.GetTasks(board, account);
        }
        public static InlineKeyboardMarkup TaskButton(Task task)
        {

            var input = task.Name;
            var keys = new List<List<InlineKeyboardButton>>();
            
            for (int i = 0; i < input.Length; i++)
            {
                var task = input[i];
                var button =
                    new InlineKeyboardButton()
                    {
                        Text = task.Name,
                        CallbackData = PackParams("get_task", board.Id.ToString(), task.Id.ToString())
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