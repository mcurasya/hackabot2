using System.Collections.Generic;
using hackabot.Client;
using hackabot.Db.Model;
using Monad;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace hackabot.Commands
{
    public class WaitForBoardNameCommand : Command
    {
        public override Response Execute(Message message, Client.Client client, Account account)
        {
            if (account.CurrentBoard != null)
            {
                account.CurrentBoard.Name = message.Text;
                //why
                //account.CurrentBoard = null;
                account.Controller.SaveChanges();
                return new Response().TextMessage(account, "Board name changed");
            }
            account.CurrentBoard = new Board
            {
                Name = message.Text,
                Owner = account,

            };
            account.Controller.AddBoard(account.CurrentBoard);
            account.Controller.SaveChanges();

            var secondCommand = new BoardCommand();
            return secondCommand.Execute(message, client, account);
        }

        public override bool Suitable(Message message, Account account) => account.Status == AccountStatus.WaitForBoardName;

    }
}