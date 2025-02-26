using System;

namespace TaskFlow.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public TaskStatus Status { get; set; }

        // Внешний ключ к пользователю
        public int UserId { get; set; }
        public User User { get; set; }
    }

    public enum TaskStatus
    {
        New,
        InProgress,
        Completed
    }
}
