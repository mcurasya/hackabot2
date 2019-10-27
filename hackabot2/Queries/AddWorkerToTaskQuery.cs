using System.Collections.Generic;
using System.Linq;
using hackabot.Db.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot.Queries
{
    public class AddWorkerToTaskQuery : Query
    {
        public override string Alias { get; } = "get_worker_list";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var board = account.Controller.GetBoard(int.Parse(values["boardId"]));
            var task = account.Controller.GetTasks(board, account).First(t => t.Id.ToString() == values["taskId"]);
            account.CurrentTask = task; 
            account.CurrentBoard = board;
            return new Response().EditMessageMarkup(account, message.Message.MessageId, WorkerButtons(task));
        }

        public static InlineKeyboardMarkup WorkerButtons(Task task)
        {
            var input = task.Board.Workers?.ToArray();
            var keys = new List<List<InlineKeyboardButton>>();

            for (int i = 0; i < input?.Length; i++)
            {
                var worker = input[i];
                var button =
                    new InlineKeyboardButton()
                    {
                        Text = worker.Worker.Username,
                        CallbackData = PackParams("add_worker", ("boardId", task.Board.Id.ToString()), ("taskId", task.Id.ToString()), ("workerId", worker.Id.ToString()))
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

    public class AddWorkerQuery : Query
    {
        public override string Alias { get; } = "add_worker";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var board = account.Controller.GetBoard(int.Parse(values["boardId"]));
            var task = account.Controller.GetTasks(board, account).First(t => t.Id.ToString() == values["taskId"]);
            var worker = board.Workers.First(t => t.Id.ToString() == values["worker_id"]);
            task.AssignedTo = worker.Worker;
            account.Controller.SaveChanges();
            return new Response().EditMessageMarkup(account.ChatId,message.Message.MessageId,EditTaskQuery.ManageTaskAdminButton(account,task,board).ToArray());
        }
    }
}