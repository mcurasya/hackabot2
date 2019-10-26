using System.ComponentModel.DataAnnotations.Schema;

namespace hackabot.Db.Model
{
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