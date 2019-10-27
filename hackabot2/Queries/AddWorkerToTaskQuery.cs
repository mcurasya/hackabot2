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
                    return new InlineKeyboardButton()
                    {
                        Text = "bananchick_pasha",
                        //CallbackData = PackParams("add_worker", ("boardId", task.Board.Id.ToString()), ("taskId", task.Id.ToString()), ("workerId", worker.Id.ToString()))
                        CallbackData = PackParams("add_worker", ("boardId", task.Board.Id.ToString()), ("taskId", task.Id.ToString()), ("workerId", "4"))
                    };
        }
    }

    public class AddWorkerQuery : Query
    {
        public override string Alias { get; } = "add_worker";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var board = account.Controller.GetBoard(int.Parse(values["boardId"]));
            var task = account.Controller.GetTasks(board, account).First(t => t.Id.ToString() == values["taskId"]);
            var worker = account.Controller.GetAccount(4);
            task.AssignedTo = worker;
            account.Controller.SaveChanges();
            return new Response().EditMessageMarkup(account.ChatId,message.Message.MessageId,EditTaskQuery.ManageTaskAdminButton(account,task,board).ToArray());
        }
    }
}