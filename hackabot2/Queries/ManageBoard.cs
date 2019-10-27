using System;
using System.Collections.Generic;
using System.Linq;
using hackabot.Db.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot.Queries
{
    public class ManagePeople : Query
    {
        public override string Alias => "manage_people";

        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {

            var board = account.Controller.GetBoard(int.Parse(values["id"]));
            //todo list people
            var text = "this is list of your personal";
            return new Response().EditTextMessage(account, message.Message.MessageId, text, ManagePeopleButtons(account, board));
        }
        public static InlineKeyboardMarkup ManagePeopleButtons(Account a, Board board)
        {
            //todo add separate logic for owner and manager

            return new InlineKeyboardMarkup(
                new InlineKeyboardButton[][]
                {
                    new InlineKeyboardButton[]
                        {
                            new InlineKeyboardButton()
                                {
                                    Text = "Add person",
                                        CallbackData = $"add_manager id={board.Id}"
                                },
                                new InlineKeyboardButton
                                {
                                    Text = "Remove Person",
                                        CallbackData = "manage_tasks id=" + board.Id
                                }
                        },
                        new InlineKeyboardButton[]
                        {
                            new InlineKeyboardButton
                            {
                                Text = "<<",
                                    CallbackData = "board_list"
                            },

                        }

                }

            );

        }
    }
    public class ManageBoard : Query
    {
        public override string Alias { get; } = "manage_board";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var board = account.Controller.GetBoard(int.Parse(values["id"]));
            //todo boards stats
            var text = $@"Board: {board.Name}
Owner: {board.Owner.Username}
";
            return new Response().EditTextMessage(account.ChatId, message.Message.MessageId, text, ManageBoardButtons(account, board));
        }

        public static InlineKeyboardMarkup ManageBoardButtons(Account a, Board board)
        {

            var buttons = new List<InlineKeyboardButton>();
            buttons.Add(
                new InlineKeyboardButton()
                {
                    Text = "View tasks",
                        CallbackData = $"get_task_list {board.Id}"
                });
            if (board.Owner == a)
            {
                buttons.Add(
                    new InlineKeyboardButton()
                    {
                        Text = "Add manager",
                            CallbackData = $"add_manager {board.Id}"
                    });
                buttons.Add(new InlineKeyboardButton()
                {
                    Text = "Change Name",
                        CallbackData = $"change_name {board.Id}"
                });
                buttons.Add(new InlineKeyboardButton()
                {
                    Text = "Create Task",
                        CallbackData = $"create_task {board.Id}"

                });
                buttons.Add(new InlineKeyboardButton()
                {
                    Text = "Edit Task",
                        CallbackData = $"edit_task {board.Id}"

                });
            }
            return buttons.ToArray();
        }
    }
    public class ChangeBoardNameQuery : Query
    {
        public override string Alias { get; } = "change_name";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            account.CurrentBoard = account.Controller.GetBoards(account).First(t => t.Id.ToString() == values.First().Value);
            account.Status = AccountStatus.WaitForBoardName;
            return new Response().TextMessage(account.ChatId, "Please enter new name");
        }

    }
    public class CreateTaskQuery : Query
    {
        public override string Alias { get; } = "create_task";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var board = account.Controller.GetBoards(account).First(t => t.Id.ToString() == values.First().Value);
            account.Status = AccountStatus.WaitForTaskName;
            return new Response().TextMessage(account.ChatId, "Please enter task name");
        }

    }
    public class AddManager : Query
    {
        public override string Alias { get; } = "add_manager";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var board = account.Controller.GetBoards(account).First(t => t.Id.ToString() == values.First().Value);
            throw new NotImplementedException();
        }

    }
}