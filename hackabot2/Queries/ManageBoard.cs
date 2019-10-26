using System;
using System.Collections.Generic;
using System.Linq;
using hackabot.Commands;
using hackabot.Db.Model;
using hackabot2.Commands;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot.Queries
{
    public class ManageBoard : Query
    {
        public override string Alias { get; } = "Edit Board";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var board = account.Controller.GetBoards(account).First(t => t.Id.ToString() == values.First().Value);
            return new QuerryResponse().EditMessageMarkup(account, message.Message.MessageId, ManageBoardButtons(account, board));
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
            return new Response(new WaitForBoardNameCommand()).TextMessage(account.ChatId, "Please enter new name");
        }

    }
    public class CreateTaskQuery : Query
    {
        public override string Alias { get; } = "create_task";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var board = account.Controller.GetBoards(account).First(t => t.Id.ToString() == values.First().Value);
            return new Response(new WaitForTaskNameCommand()).TextMessage(account.ChatId, "Please enter task name");
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