using System.Collections.Generic;
using hackabot.Client;
using hackabot.Db.Model;
using Monad;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace hackabot.Commands
{
    public class WaitForBoardNameCommand : InputCommand
    {
        public override MessageType[] InputTypes => new [] { MessageType.Text };

        protected override Response Run(Message message, Client.Client client, Account account, EitherStrict<ICommand, IEnumerable<IOneOfMany>> prevCommands)
        {
            account.CurrentBoard = new Board()
            {
                Name = message.Text,
                Owner = account
            };
            return new Response(prevCommands).TextMessage(account, "New board created"); //todo board text + menu 
        }
    }
}