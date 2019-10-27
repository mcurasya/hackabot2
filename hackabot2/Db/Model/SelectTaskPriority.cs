using System.Collections.Generic;
using hackabot.Db.Model;
using Telegram.Bot.Types;

namespace hackabot.Queries
{
    public class SelectTaskPriority : Query
    {
        public override string Alias =>
            "select_task_priority";

        protected override Response Run(CallbackQuery message, Account account, Dictionary<string, string> values)
        {
            if (account.CurrentTask != null)
            {
                account.CurrentTask.Priority = (Priorities) int.Parse(values["priority"]);
                //account.Controller.AddTask(account.CurrentTask);
                account.Controller.SaveChanges();
                account.Status = AccountStatus.WaitingForTaskDescription;
                return new Response().EditTextMessage(account, message.Message.MessageId, "Enter task description");
            }
            return new Response().TextMessage(account, "You don't have this task"); //todo board text + menu 
        }
    }
}