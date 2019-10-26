using System;
using System.Collections.Generic;
using System.Text;
using hackabot.Db.Model;
using Monad;
using Telegram.Bot.Types;

namespace hackabot.Commands
{
    public class StartCommand : StaticCommand
    {
        public override string Alias { get; } = "/start";

        public static string InviteUser(Board board, Account inviter, AccessLevel accessLevel)
        {
            //board id | inviter id | accesslevel
            return $"{board.Id}*{inviter.Id}*{((int)accessLevel)}";
        }
        public override Response Execute(Message message, Client.Client client, Account account, EitherStrict<ICommand, IEnumerable<IOneOfMany>> prevCommands)
        {

            if (message.Text.Length > "/start ".Length)
            {
                var param = message.Text.Substring(7);
                var base64EncodedBytes = Convert.FromBase64String(param);
                param = Encoding.UTF8.GetString(base64EncodedBytes);
                var p = param.Split('*');

                var board = account.Controller.GetBoard(int.Parse(p[0]));
                account.Controller.AssignWorkerToBoard(account, board);
                //todo start command
            }

            return new Response(prevCommands).TextMessage(account.ChatId, "hello n");
        }
    }
}