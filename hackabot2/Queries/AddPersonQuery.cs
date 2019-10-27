using System.Collections.Generic;
using hackabot.Commands;
using hackabot.Db.Model;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot.Queries
{
    public class SelectPersonType : Query
    {
        public override string Alias => "send_person_name";

        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            var id = int.Parse(values["id"]);
            var board = account.Controller.GetBoard(id);
            var type = values["type"] == "1" ? AccessLevel.Manager : AccessLevel.Worker;
            return new Response().TextMessage(account, $"This is an invitation to {board.Name} as " + (type == AccessLevel.Manager? "mananger": "worker") + "\n" + StartCommand.InviteUser(board, account, type));
        }
    }
    public class AddPersonQuery : Query
    {
        public override string Alias => "add_person";

        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {

            return new Response().EditTextMessage(account, message.Message.MessageId, "Choose person type:", AddPerson(values["id"]));
        }

        public static InlineKeyboardMarkup AddPerson(string Id)
        {
            return new InlineKeyboardMarkup(
                new InlineKeyboardButton[][]
                {
                    new InlineKeyboardButton[]
                        {
                            new InlineKeyboardButton()
                                {
                                    Text = "Add manager",
                                        CallbackData = $"send_person_name id={Id}&type=1"
                                },
                                new InlineKeyboardButton
                                {
                                    Text = "Add worker",
                                        CallbackData = $"send_person_name id={Id}&type=2"
                                }
                        },
                        new InlineKeyboardButton[]
                        {
                            new InlineKeyboardButton
                            {
                                Text = "<<",
                                    CallbackData = "get_board id=" + Id
                            },

                        }
                }

            );
        }
    }
}