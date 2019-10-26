using System;
using System.Text;
using hackabot.Db.Model;
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
        public override Response Execute(Message message, Client.Client client, Account account)
        {
            if (message.Text.Length < "/start ".Length) return new Response().TextMessage(account.ChatId, "Hi! You have no boards.\nUse buttons to create new or ask your manager to send you invite link.", Keyboards.CreateBoards(account));

            var param = message.Text.Substring(7);
            var base64EncodedBytes = Convert.FromBase64String(param);
            param = Encoding.UTF8.GetString(base64EncodedBytes);
            var p = param.Split('*');

            var board = account.Controller.GetBoard(int.Parse(p[0]));
            var accessLevel = (AccessLevel) int.Parse(p[2]);
            account.Controller.AddWorkerToBoard(new WorkerToBoard()
            {
                Worker = account,
                    Board = board,
                    AccessLevel = accessLevel
            });

            return new Response().TextMessage(account, $"You we're added to {board.Name}");
        }
    }
}