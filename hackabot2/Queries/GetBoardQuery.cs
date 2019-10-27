using System;
using System.Collections.Generic;
using System.Linq;
using hackabot.Db.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot.Queries
{
    public class GetBoardQuerry : Query
    {
        public override string Alias { get; } = "get_board";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            int boardId = int.Parse(values["id"]);
            try
            {
                var board = account.Controller.GetBoard(boardId);
               var text = $@"Board: {board.Name}
Owner: {board.Owner.Username}
";
                return new Response().EditTextMessage(account.ChatId, message.Message.MessageId, text, ManageBoard(board));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Response().TextMessage(account.ChatId, "No such board");
            }
        }
        //for user
        public static InlineKeyboardMarkup ViewBoard(Board b)
        {
            return new InlineKeyboardButton[]
            {
                new InlineKeyboardButton
                {
                    Text = "<<",
                        CallbackData = "board_list"
                },
                new InlineKeyboardButton
                {
                    Text = "Tasks",
                        CallbackData = "manage_tasks id=" + b.Id
                }
            };
        }
        //for admin
        public static InlineKeyboardMarkup ManageBoard(Board b)
        {
            return new InlineKeyboardMarkup(
                new InlineKeyboardButton[][]
                {
                    new InlineKeyboardButton[]
                        {
                            new InlineKeyboardButton
                            {
                                Text = "Edit Board",
                                    CallbackData = "manage_board id=" + b.Id
                            },
                            new InlineKeyboardButton
                            {
                                Text = "Manage People",
                                    CallbackData = "manage_people id=" + b.Id
                            }
                        },
                        new InlineKeyboardButton[]
                        {
                            new InlineKeyboardButton
                            {
                                Text = "<<",
                                    CallbackData = "board_list"
                            },
                            new InlineKeyboardButton
                            {
                                Text = "Manage Tasks",
                                    CallbackData = "manage_tasks id=" + b.Id
                            }
                        }

                }

            );
        }
    }
    public class GetTaskListQuery : Query
    {
        public override string Alias { get; } = "get_task_list";
        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            //aaah i dont know why it does not work
            int boardId = int.Parse(values["id"]);
            var board = account.Controller.GetBoards(account).FirstOrDefault(t=>t.Id == boardId);
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
            keys.Add(new List<InlineKeyboardButton>{new InlineKeyboardButton
            {
                Text = "<<",
                CallbackData = $"get_board id {board.Id}"
            }});
            return keys.ToArray();
        }
    }
}