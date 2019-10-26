using System.Collections.Generic;
using hackabot.Client;
using hackabot.Db.Model;
using Monad;
using Telegram.Bot.Types;

namespace hackabot.Commands
{
    public class BoardCommand : Command
    {
        public override Response Execute(Message message, Client.Client client, Account account)
        {
            var boards = account.Controller.GetBoards(account);

            return new Response().TextMessage(account, $"You have {boards.Length} boards.\nChoose one board to get more details:",
                Keyboards.BoardsKeyboard(boards));
        }

        public override bool Suitable(Message message, Account account) =>
            message.Text.StartsWith("/boards");
    }
}