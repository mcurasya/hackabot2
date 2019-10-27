using System;
using System.Linq;
using Tasks = System.Threading.Tasks;
using hackabot.Db.Model;
using hackabot2.Db.Controllers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Task = hackabot.Db.Model.Task;
using TaskStatus = hackabot.Db.Model.TaskStatus;

namespace hackabot.Notifications
{
    public class NotificationManager
    {
        TelegramController controller;
        static Client.Client _client;

        public NotificationManager(Client.Client client)
        {
            controller = new TelegramController();
            controller.Start();
            _client = client;
        }
        
        public async Tasks.Task NotifyDaysLeft(Task task)
        {
            var messageText = $"you have {(task.EndDate.Date - DateTime.Today).Days} left to close task {task.Name}, keep going, you are still better then @bananchik_pasha";
            await _client.SendTextMessageAsync(task.AssignedTo, messageText);
        }
        public static async Tasks.Task NewTask(Account account, Task t, Board b)
        {
            var endTaskDate = new DateTime(2019,8,27,16,13,0);
            var timespan = DateTime.Now - endTaskDate;
            account.CurrentTask = t;
            account.CurrentBoard = b;
            var text = $@"You have new task!
Priority:${t.Priority}
Days left${t.EndDate - DateTime.Now}
.....................
It is more important to do Make Presentation Task. You had to close it ${timespan.Minutes} ago!
How it is?";
            var buttons = new ReplyKeyboardMarkup(new KeyboardButton[]
            {
                "I did it!",
                "Just one moment...",
                "Oh no, i failed it :("
            });
            account.Status = AccountStatus.TaskPresentationStatus;
            await _client.SendTextMessageAsync(account, text, replyMarkup: buttons);
            
        }

        public async Tasks.Task NotifyExpired(Task task)
        {
            var messageText = $"you have expired your task {task.Name} for {(DateTime.Today - task.EndDate.Date).Days}, please close it already";
            await _client.SendTextMessageAsync(task.AssignedTo, messageText);
        }

        public async void Cycle()
        {
            await Tasks.Task.Run(async () =>
            {
                while (false)
                {
                    foreach (var task in controller.Context.Tasks.Where(task =>
                        (task.EndDate.Date - DateTime.Today).Days <= 7 &&
                        (task.EndDate.Date - DateTime.Today).Days > 0))
                    {
                        await NotifyDaysLeft(task);
                    }
                    foreach (var task in controller.Context.Tasks.Where(task =>
                        (task.EndDate.Date - DateTime.Today).Days < 0))
                    {
                        await NotifyExpired(task);
                    }
                    await Tasks.Task.Delay(new TimeSpan(0, 0,1, 0));
                }
            });

        }
    }
}