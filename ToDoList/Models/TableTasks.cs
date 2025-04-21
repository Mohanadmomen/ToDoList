namespace ToDoList.Models
{
    public class TableTasks
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
        public string Title { get; set; }

        public string? Content { get; set; }

        public bool IsCompleted { get; set; }

    }
}
