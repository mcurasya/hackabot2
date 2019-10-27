using System;
using System.Collections.Generic;
using System.Linq;
using hackabot.Db.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot.Queries
{
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
            return new InlineKeyboardButton[]
            {
                new InlineKeyboardButton
                {
                    Text = "<<",
                        CallbackData = "get_board id=" + board.Id.ToString()
                },
                new InlineKeyboardButton()
                    {
                        Text = "Change Name",
                            CallbackData = PackParams("change_board_name", "id", board.Id.ToString())
                    },

            };

            // buttons.Add(
            //     new InlineKeyboardButton()
            //     {
            //         Text = "View tasks",
            //         CallbackData = PackParams("get_task_list", "id", board.Id.ToString())
            //     });
            // if (board.Owner == a)
            // {
            //     buttons.Add(
            //         new InlineKeyboardButton()
            //         {
            //             Text = "Add manager",
            //                 CallbackData = PackParams("add_manager", "id", board.Id.ToString())
            //         });

            // buttons.Add(new InlineKeyboardButton()
            // {
            //     Text = "Create Task",
            //     CallbackData = PackParams("create_task", "id", board.Id.ToString())

            // });
            // buttons.Add(new InlineKeyboardButton()
            // {
            //     Text = "Edit Task",
            //     CallbackData = PackParams("edit_task", "id", board.Id.ToString())

            // });
        }

    }

    public class ChangeBoardNameQuery : Query
    {
        public override string Alias { get; } = "change_board_name";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            account.CurrentBoard = account.Controller.GetBoards(account).First(t => t.Id.ToString() == values.First().Value);
            account.Status = AccountStatus.WaitForBoardName;
            return new Response().EditTextMessage(account.ChatId, message.Message.MessageId, "Please enter new name");
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