using hackabot.Db.Model;
using Telegram.Bot.Types;

namespace hackabot.Commands
{
    public class HelpCommand : StaticCommand
    {
        public override string Alias => "/help";

        public override Response Execute(Message message, Client.Client client, Account account)
        {
            return new Response().TextMessage(account, "help");
        }
    }
}