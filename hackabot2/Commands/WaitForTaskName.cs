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
    public class WaitForTaskPrioriyCommand : Command
    {
        public override Response Execute(Message message, Client client, Account account)
        {
            if (account.CurrentTask != null)
            {
                var prior = account.CurrentTask.Priority;
                var text = "Task priority " + (prior == Priorities.none ? "Added" : "Changed");
                if (!Enum.TryParse(message.Text, out prior))
                {
                    return new Response().TextMessage(account, "Invalid input");
                }

                account.CurrentTask.Priority = prior;
                account.CurrentTask = null;
                account.Controller.SaveChanges(); // will it work?
                account.Status = AccountStatus.Free;
                return new Response().TextMessage(account, text); //todo board text + menu 
            }
            return new Response().TextMessage(account, "You don't have this task"); //todo board text + menu 
        }

        public override bool Suitable(Message message, Account account) =>
            account.Status == AccountStatus.WaitingForTaskPriority;

    }
    public class WaitForTaskDescriptionCommand : Command
    {
        public override Response Execute(Message message, Client client, Account account)
        {
            if (account.CurrentTask != null)
            {
                var text = "Task description " + (account.CurrentTask.Description == "" | account.CurrentTask.Description == null ? "Added" : "Changed");
                account.CurrentTask.Description = message.Text;
                account.Status = AccountStatus.Free;
                account.Controller.SaveChanges();
            return new Response().TextMessage(account, text); //todo board text + menu 
            }
            return new Response().TextMessage(account, "You don't have this task"); //todo board text + menu 
        }

        public override bool Suitable(Message message, Account account) =>
            account.Status == AccountStatus.WaitingForTaskPriority;

    }
    public class WaitForTaskStatus : Command
    {
        public override Response Execute(Message message, Client client, Account account)
        {
            if (account.CurrentTask != null)
            {
                //бля треба би maybe монаду заїбашити але мені впадла
                //і єбать тут темлпейтного коду
                var status = account.CurrentTask.Status;
                var text = "Task status " + (status == TaskStatus.none ? "Added" : "Changed");
                if (!Enum.TryParse(message.Text, out status))
                {
                    return new Response().TextMessage(account, "Invalid input");
                }

                account.CurrentTask.Status = status;
                account.CurrentTask = null;
                account.Controller.SaveChanges(); // will it work?
                account.Status = AccountStatus.Free;
                return new Response().TextMessage(account, text); //todo board text + menu 
            }
            return new Response().TextMessage(account, "You don't have this task"); //todo board text + menu 
        }

        public override bool Suitable(Message message, Account account) =>
            account.Status == AccountStatus.WaitingForTaskStatus;

    }

    public class WaitForTaskEndDate : Command
    {
        public override Response Execute(Message message, Client client, Account account)
        {
            if (account.CurrentTask != null)
            {
                var text = "Task date " +
                           (account.CurrentTask.Description == "" | account.CurrentTask.Description == null
                               ? "Added"
                               : "Changed");
                try
                {
                    account.CurrentTask.EndDate = DateTime.Parse(message.Text);
                }
                catch (FormatException e)
                {
                    return new Response().TextMessage(account, "invalid date!");
                }

                account.Status = AccountStatus.Free;
                account.Controller.SaveChanges();
                return new Response().TextMessage(account, text); //todo board text + menu 
            }

            return new Response().TextMessage(account, "You don't have this task"); //todo board text + menu 
        }

        public override bool Suitable(Message message, Account account) =>
            account.Status == AccountStatus.WaitingForTaskEndTime;
    }
}