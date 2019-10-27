using System.Linq;
using hackabot;
using hackabot.Client;
using hackabot.Commands;
using hackabot.Db.Model;
using Telegram.Bot.Types;

namespace hackabot2.Commands
{
    public class ChangeTaskStatusCommand : Command
    {
        public override bool Suitable(Message message, Account account) =>
            account.Status == AccountStatus.TaskPresentationStatus;

        public override Response Execute(Message message, Client client, Account account)
        {
            if (message.Text == "I did it!")
            {
                account.CurrentTask.Status = TaskStatus.Done;
                account.Controller.SaveChanges();
                var task = account.Controller.GetTasks(account.CurrentBoard, account).First();
                account.Status = AccountStatus.Free;
                return new Response().TextMessage(account.ChatId, $"Good job! New the most important task is ${task.Name}");
            }
            if (message.Text == "Just one moment...")
            {
                account.CurrentTask.Status = TaskStatus.Testing;
                account.Controller.SaveChanges();
                account.Status = AccountStatus.Free;
                return new Response().TextMessage(account.ChatId, "Okay, please be faster");
            }
            else
            {
                account.Status = AccountStatus.Free;
                return new Response().TextMessage(account.ChatId, "What a shame! Finish it as fast as you can!");
            }
        }
    
    }
}