using System.Collections.Generic;
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

        [NotMapped]
        public TelegramController Controller { get; set; }
        public static implicit operator ChatId(Account a) => a.ChatId;

    }
    public class Board
    {
        public int Id { get; set; }
        public string Name { get; set; } 

        [ForeignKey("OwnerId")]
        public Account Owner { get; set; }

        public List<WorkerToBoard> Workers { get; set; }
        public List<Task> Tasks { get; set; }

    }
    public class WorkerToBoard
    {
        public int Id { get; set; }

        [ForeignKey("WorkerId")]
        public Account Worker { get; set; }

        [ForeignKey("BoardId")]
        public Board Board { get; set; }

        public AccessLevel AccessLevel { get; set; }
    }
}