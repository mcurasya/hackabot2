using System;
using System.Collections.Generic;
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
            Console.WriteLine(boardId);
            try
            {
                var board = account.Controller.GetBoard(boardId);
                var text = $@"Board: {board.Name}
Owner: {board.Owner?.Username}
{account.Controller.GetStatAboutBoard(board)}";
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
}