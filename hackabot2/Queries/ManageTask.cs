using System;
using System.Collections.Generic;
using System.Linq;
using hackabot.Commands;
using hackabot.Db.Model;
using hackabot2.Commands;
using Monad.Parsec;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot.Queries
{
    public class EditTaskQuery : Query
    {
        public override string Alias { get; } = "edit_task";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var board = account.Controller.GetBoards(account).First(t => t.Id.ToString() == values.First().Key);
            var task = account.Controller.GetTasks(board, account).First(t => t.Id.ToString() == values.First().Value);
            var buttons = new List<InlineKeyboardButton>();
            if(task.Creator == account || board.Owner == account)
                buttons.AddRange(ManageTaskAdminButton(account,task,board));
            if(task.AssignedTo == account)
                buttons.AddRange(ManageTaskWorkerButton(account,task,board));
            var text = $@"Task: {task.Name}
Description: {task.Description}
Estimated Date: {task.EstimatedTime}
Priority: {task.Priority}
Status: {task.Status}
";
            return new QuerryResponse().EditMessageMarkup(account, message.Message.MessageId, buttons.ToArray()).EditTextMessage(account.ChatId, message.Message.MessageId, text);;
        }
        public static List<InlineKeyboardButton> ManageTaskAdminButton(Account a, Task task, Board board)
        {
            var buttons = new List<InlineKeyboardButton>();
                buttons.Add(
                    new InlineKeyboardButton()
                    {
                        Text = "Add worker",
                        CallbackData = $"add_worker_task {board.Id} {task.Id}"
                    });
                buttons.Add(
                    new InlineKeyboardButton()
                    {
                        Text = "Change Name",
                        CallbackData = $"change_name_task {board.Id} {task.Id}"
                    });
                buttons.Add(
                    new InlineKeyboardButton()
                    {
                        Text = "Change Priority",
                        CallbackData = $"change_prior_task {board.Id} {task.Id}"
                    });
            {
            }
            return buttons;
        }

        public static List<InlineKeyboardButton> ManageTaskWorkerButton(Account a, Task task, Board board)
        {
            var buttons = new List<InlineKeyboardButton>();
                buttons.Add(
                    new InlineKeyboardButton()
                    {
                        Text = "Mark as done",
                        CallbackData = $"done_task {board.Id} {task.Id}"
                    });
                buttons.Add(
                    new InlineKeyboardButton()
                    {
                        Text = "Change status",
                        CallbackData = $"change_status_task {board.Id} {task.Id}"
                    });
                buttons.Add( //додамо купу полів типу у нас дохуя функціоналу
                    new InlineKeyboardButton()
                    {
                        Text = "Add note",
                        CallbackData = $"add_note_task {board.Id} {task.Id}"
                    });
            return buttons;
        }
        
    public class ChangeTaskNameQuery : Query
    {
        public override string Alias { get; } = "change_name_task";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            account.CurrentBoard = account.Controller.GetBoards(account).First(t => t.Id.ToString() == values.First().Key);
            account.CurrentTask = account.Controller.GetTasks(account.CurrentBoard, account).First(t => t.Id.ToString() == values.First().Value);
            return new Response(new WaitForTaskNameCommand()).TextMessage(account.ChatId, "Please enter new name");
        }

    }

    }
}