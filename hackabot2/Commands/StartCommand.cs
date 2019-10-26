using System.Collections.Generic;
using hackabot.Db.Model;
using Monad;
using Telegram.Bot.Types;

namespace hackabot.Commands
{
    public class StartCommand : StaticCommand
    {
        public override string Alias { get; } = "/start";

        public override Response Execute(Message message, Client.Client client, Account account, EitherStrict<ICommand, IEnumerable<IOneOfMany>> prevCommands)
        {
            return new Response(prevCommands).TextMessage(account.ChatId, "hello n");
        }
    }
}