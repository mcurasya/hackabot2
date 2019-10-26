using hackabot.Db.Model;
using Telegram.Bot.Types;

namespace hackabot.Commands
{
    public abstract class Command
    {
        public abstract bool Suitable(Message message, Account account);
        public abstract Response Execute(Message message, Client.Client client, Account account);
    }
    public abstract class StaticCommand : Command
    {
        public abstract string Alias { get; }
        public override bool Suitable(Message message, Account account) => message.Text.StartsWith(Alias);
    }
}