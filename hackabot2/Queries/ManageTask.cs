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
                    Text = "Add note",//what is note???
                        CallbackData = $"add_note_task {board.Id} {task.Id}"
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
                account.CurrentBoard = account.Controller.GetBoards(account).First(t => t.Id.ToString() == values.First().Key);
                account.CurrentTask = account.Controller.GetTasks(account.CurrentBoard, account).First(t => t.Id.ToString() == values.First().Value);
                return new Response().TextMessage(account.ChatId, "Please enter new name");
            }

        }

        public class ChangeTaskPriorityQuery : Query
        {
            public override string Alias => "change_prior_task";
            protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
            {//todo add markup
                account.CurrentBoard = account.Controller.GetBoards(account).First(t => t.Id.ToString() == values.First().Key);
                account.CurrentTask = account.Controller.GetTasks(account.CurrentBoard, account).First(t => t.Id.ToString() == values.First().Value);
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
                account.CurrentBoard = account.Controller.GetBoards(account).First(t => t.Id.ToString() == values.First().Key);
                account.CurrentTask = account.Controller.GetTasks(account.CurrentBoard, account).First(t => t.Id.ToString() == values.First().Value);
                return new Response().TextMessage(account.ChatId, "Please enter new name");
            }
        }
        
        public class ChangeTaskStatus : Query
        {
            public override string Alias => "change_status_task";
            protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
            {
                account.CurrentBoard = account.Controller.GetBoards(account).First(t => t.Id.ToString() == values.First().Key);
                account.CurrentTask = account.Controller.GetTasks(account.CurrentBoard, account).First(t => t.Id.ToString() == values.First().Value);
                return new Response().TextMessage(account.ChatId, "Please enter new name");
            }
        }
        
        public class ChangeTaskEndDate : Query
        {
            public override string Alias => "change_enddate_task";
            protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
            {
                account.CurrentBoard = account.Controller.GetBoards(account).First(t => t.Id.ToString() == values.First().Key);
                account.CurrentTask = account.Controller.GetTasks(account.CurrentBoard, account).First(t => t.Id.ToString() == values.First().Value);
                return new Response().TextMessage(account.ChatId, "Please enter new name");
            }
        }
        
    }
}