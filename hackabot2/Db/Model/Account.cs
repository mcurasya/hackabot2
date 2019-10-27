using System.ComponentModel.DataAnnotations.Schema;
using hackabot2.Db.Controllers;
using Telegram.Bot.Types;

namespace hackabot.Db.Model
{
    public class Account
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string Username { get; set; }
        public AccountStatus Status { get; set; }

        [NotMapped]
        public TelegramController Controller { get; set; }

        [NotMapped]
        public Board CurrentBoard { get; set; }

        [NotMapped]
        public Task CurrentTask { get; set; }
        public static implicit operator ChatId(Account a) => a.ChatId;

    }
    public enum AccountStatus
    {
        Start,
        Free,
        WaitForBoardName,
        WaitForTaskName,
        WaitingForTaskDescription,
        WaitingForTaskStatus,
        WaitingForTaskAssigned,
        WaitingForTaskEndTime,
        WaitingForTaskPriority,
        
    }
}