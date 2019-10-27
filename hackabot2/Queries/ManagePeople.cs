using System.Collections.Generic;
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
                                        CallbackData = $"add_person id={board.Id}"
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
                                    CallbackData = "get_board id=" + board.Id
                            },

                        }
                }

            );

        }
    }
}