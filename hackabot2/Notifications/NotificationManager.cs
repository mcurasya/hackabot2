using System;
using System.Linq;
using Tasks = System.Threading.Tasks;
using hackabot.Db.Model;
using hackabot2.Db.Controllers;
using Telegram.Bot.Types;
using Task = hackabot.Db.Model.Task;
using TaskStatus = hackabot.Db.Model.TaskStatus;

namespace hackabot.Notifications
{
    public class NotificationManager
    {
        TelegramController controller;
        Client.Client _client;

        public NotificationManager(Client.Client client)
        {
            controller = new TelegramController();
            controller.Start();
            _client = client;
        }
        
        public async Tasks.Task NotifyDaysLeft(Task task)
        {
            var messageText = $"you have {(task.EndDate.Date - DateTime.Today).Days} left to close task {task.Name}";
            await _client.SendTextMessageAsync(task.AssignedTo, messageText);
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
                while (true)
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
                    await Tasks.Task.Delay(new TimeSpan(1, 0, 0, 0));
                }
            });

        }
    }
}