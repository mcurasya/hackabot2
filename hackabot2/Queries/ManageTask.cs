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
            if (task.Creator == account || board.Owner == account)
                buttons.AddRange(ManageTaskAdminButton(account, task, board));
            if (task.AssignedTo == account)
                buttons.AddRange(ManageTaskWorkerButton(account, task, board));
            var text = $@"Task: {task.Name}
Description: {task.Description}
Estimated Date: {task.EstimatedTime}
Priority: {task.Priority}
Status: {task.Status}
";
            return new Response().EditMessageMarkup(account, message.Message.MessageId, buttons.ToArray()).EditTextMessage(account.ChatId, message.Message.MessageId, text);
        }
        public static List<InlineKeyboardButton> ManageTaskAdminButton(Account a, Task task, Board board)
        {
            var buttons = new List<InlineKeyboardButton>();
            buttons.Add(
                new InlineKeyboardButton()
                {
                    Text = "Add worker",
                    CallbackData = PackParams("add_worker_task", ("boardId", board.Id.ToString()), ("taskId", task.Id.ToString()))
                });
            buttons.Add(
                new InlineKeyboardButton()
                {
                    Text = "Change Name",
                    CallbackData = PackParams("change_name_task", ("boardId", board.Id.ToString()), ("taskId", task.Id.ToString()))
                });
            buttons.Add(
                new InlineKeyboardButton()
                {
                    Text = "Change Priority",
                    CallbackData = PackParams("change_prior_task", ("boardId", board.Id.ToString()), ("taskId", task.Id.ToString()))
                }); { }
            return buttons;
        }

        public static List<InlineKeyboardButton> ManageTaskWorkerButton(Account a, Task task, Board board)
        {
            var buttons = new List<InlineKeyboardButton>();
            buttons.Add(
                new InlineKeyboardButton()
                {
                    Text = "Mark as done",
                    CallbackData = PackParams("done_task", ("boardId", board.Id.ToString()), ("taskId", task.Id.ToString()))
                });
            buttons.Add(
                new InlineKeyboardButton()
                {
                    Text = "Change status",
                    CallbackData = PackParams("change_status_task", ("boardId", board.Id.ToString()), ("taskId", task.Id.ToString()))
                });
            return buttons;
        }

        public static List<KeyboardButton> ChangePriorityButton(Account a, Task task, Board board)
        {
            var buttons = new List<KeyboardButton>
            {
                new KeyboardButton {Text = Priorities.Low.ToString()},
                new KeyboardButton {Text = Priorities.Medium.ToString()},
                new KeyboardButton {Text = Priorities.High.ToString()},
                new KeyboardButton {Text = Priorities.Critical.ToString()}
            };
                //todo add callback data
            return buttons;
        }
        

        public static List<KeyboardButton> ChangeStatusButton(Account a, Task task, Board board)// мб не так поправь плс, эта клавиатура, которая под строкой ввода текста (аналогично с тем, что выше)
        {
            var buttons = new List<KeyboardButton>
            {
                new KeyboardButton { Text = TaskStatus.TODO.ToString()},
                new KeyboardButton { Text = TaskStatus.InProgress.ToString()},
                new KeyboardButton { Text = TaskStatus.Testing.ToString()},
                new KeyboardButton { Text = TaskStatus.Done.ToString()}
            };
            return buttons;
        }
        
        public class ChangeTaskNameQuery : Query
        {
            public override string Alias { get; } = "change_name_task";
            protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
            {
                account.CurrentBoard = account.Controller.GetBoards(account)
                    .First(t => t.Id.ToString() == values["boardId"]);
                account.CurrentTask = account.Controller.GetTasks(account.CurrentBoard, account).First(t => t.Id.ToString() == values["taskId"]);
                return new Response().TextMessage(account.ChatId, "Please enter new name");
            }

        }

        public class ChangeTaskPriorityQuery : Query
        {
            public override string Alias => "change_prior_task";
            protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
            {//todo add markup
                account.CurrentBoard = account.Controller.GetBoards(account)
                    .First(t => t.Id.ToString() == values["boardId"]);
                account.CurrentTask = account.Controller.GetTasks(account.CurrentBoard, account).First(t => t.Id.ToString() == values["taskId"]);
                account.Status = AccountStatus.WaitingForTaskPriority;
                return new Response().TextMessage(account.ChatId, "please choose new priority",
                    Priority(account, account.CurrentTask));
            }

            public static ReplyKeyboardMarkup Priority(Account a, Task task)
            {
              var buttons = Enum.GetValues(typeof(Priorities)).Cast<Priorities>()
                  .Where(t=>t != task.Priority)
                  .Select(t=> new List<KeyboardButton>() {t.ToString()});
              return new ReplyKeyboardMarkup(buttons);
            }
        }
        
        public class ChangeTaskDescriptionQuery : Query
        {
            public override string Alias => "change_description_task";
            protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
            {
                account.CurrentBoard = account.Controller.GetBoards(account)
                    .First(t => t.Id.ToString() == values["boardId"]);
                account.CurrentTask = account.Controller.GetTasks(account.CurrentBoard, account).First(t => t.Id.ToString() == values["taskId"]);
                account.Status = AccountStatus.WaitingForTaskDescription;
                return new Response().TextMessage(account.ChatId, "Please enter new name");
            }
        }
        
        public class ChangeTaskStatus : Query
        {
            public override string Alias => "change_status_task";
            protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
            {
                account.CurrentBoard = account.Controller.GetBoards(account)
                    .First(t => t.Id.ToString() == values["boardId"]);
                account.CurrentTask = account.Controller.GetTasks(account.CurrentBoard, account).First(t => t.Id.ToString() == values["taskId"]);
                account.Status = AccountStatus.WaitingForTaskStatus;
                return new Response().TextMessage(account.ChatId, "What is new status?", Status(account, account.CurrentTask));
            }
            public static ReplyKeyboardMarkup Status(Account a, Task task)
            {
              var buttons = Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>()
                  .Where(t=>t != task.Status)
                  .Select(t=> new List<KeyboardButton> {t.ToString()});
              return new ReplyKeyboardMarkup(buttons);
            }
        }
        
        public class ChangeTaskEndDate : Query
        {
            public override string Alias => "change_enddate_task";
            protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
            {
                account.CurrentBoard = account.Controller.GetBoards(account)
                    .First(t => t.Id.ToString() == values["boardId"]);
                account.CurrentTask = account.Controller.GetTasks(account.CurrentBoard, account).First(t => t.Id.ToString() == values["taskId"]);
                account.Status = AccountStatus.WaitingForTaskEndTime;
                return new Response().TextMessage(account.ChatId, "Please enter deadline date");
            }
        }
        
    }
}