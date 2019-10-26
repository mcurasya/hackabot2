
using System;

namespace hackabot2.Db.Model
{
    public class Task
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public Account Creator { get; set; }
        public Account AssignedTo { get; set; }
        public Priorities Priority { get; set; }
        public TaskStatus Status { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime EndDate { get; set; }
        public int EstimatedTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int HoursSpent { get; set; }
    }
}