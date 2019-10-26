using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.Bot.Types;

namespace hackabot
{
    public class Account
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string Username { get; set; }
        public static implicit operator ChatId(Account a) => a.ChatId;

    }
    public class Board
    {
        public int Id { get; set; }

        [ForeignKey("OwnerId")]
        public Account Owner { get; set; }

        public List<Account> Workers { get; set; }
        public List<Task> Tasks { get; set; }

    }
    public class WorkerToBoard
    {
        [ForeignKey("WorkerId")]
        public Account Worker { get; set; }

        [ForeignKey("BoardId")]
        public Board Board { get; set; }

        public AccessLevel AccessLevel { get; set; }
    }
}