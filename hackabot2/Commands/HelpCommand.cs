using System.Collections.Generic;
using hackabot.Db.Model;
using Monad;
using Telegram.Bot.Types;

namespace hackabot.Commands
{
    public class HelpCommand : StaticCommand
    {
        public override string Alias => "/help";
        public override Response Execute(Message message, Client.Client client, Account account, EitherStrict<ICommand, IEnumerable<IOneOfMany>> prevCommands)
        {
            return new Response(prevCommands).TextMessage(account, "help");
        }
    }
}