using System;
using System.Collections.Generic;
using hackabot;
using hackabot.Client;
using hackabot.Commands;
using hackabot.Db.Model;
using Monad;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace hackabot2.Commands
{
    public class WaitForTaskNameCommand : Command
    {
        public override Response Execute(Message message, Client client, Account account)
        {
            if (account.CurrentTask != null)
            {
                account.CurrentTask.Name = message.Text;
                account.CurrentTask = null;
                account.Controller.SaveChanges(); // will it work?
                account.Status = AccountStatus.Free;
                return new Response().TextMessage(account, "Task name changed"); //todo board text + menu 
            }
            account.CurrentTask = new Task()
            {
                Name = message.Text,
                Creator = account
            };
            account.Controller.AddTask(account.CurrentTask);
            account.Controller.SaveChanges();
            account.Status = AccountStatus.Free;
            return new Response().TextMessage(account, "New task created"); //todo board text + menu 
        }

        public override bool Suitable(Message message, Account account) =>
            account.Status == AccountStatus.WaitForTaskName;

    }
    public class WaitForPrioriyCommand : Command
    {
        public override Response Execute(Message message, Client client, Account account)
        {
            if (account.CurrentTask != null)
            {
                account.CurrentTask.Name = message.Text;
                var prior = account.CurrentTask.Priority;
                if (!Enum.TryParse("Active", out prior))
                {
                    return new Response().TextMessage(account, "Invalid input");
                }

                account.CurrentTask.Priority = prior;
                account.CurrentTask = null;
                account.Controller.SaveChanges(); // will it work?
                account.Status = AccountStatus.Free;
                return new Response().TextMessage(account, "Task priority changed"); 
            }
            return new Response().TextMessage(account, "You don't have this task"); //todo board text + menu 
        }

        public override bool Suitable(Message message, Account account) =>
            account.Status == AccountStatus.WaitingForTaskPriority;

    }
}