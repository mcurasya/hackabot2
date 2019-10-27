using System;
using hackabot;
using hackabot.Db.Model;
using Telegram.Bot.Types.ReplyMarkups;

namespace hackabot2.Commands.WorkerCommands
{
    public class Notifications
    {
        public static Response NewTask(Account account, Task t)
        {
            var endTaskDate = new DateTime(2019,8,27,16,13,0);
            var timespan = DateTime.Now - endTaskDate;
            var text = $@"You have new task!
Priority:${t.Priority}
Days left${t.EndDate - DateTime.Now}
.....................
It is more important to do Make Presentation Task. You had to close it ${timespan.Minutes} ago!
How it is?";
            return new Response().TextMessage(account.ChatId, text);
        }
        public static ReplyKeyboardMarkup DialogButtons(Account account)
        {
            return new ReplyKeyboardMarkup(new KeyboardButton[]
            {
                "Boards",
            });
        }
    }
    
}