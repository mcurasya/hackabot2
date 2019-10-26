using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace hackabot.Db.Model
{
    public class Board
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("OwnerId")]
        public Account Owner { get; set; }

        public List<WorkerToBoard> Workers { get; set; }
        public List<Task> Tasks { get; set; }

    }
}