namespace ToDoList.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        // Navigation property
        public ICollection<TableTasks>? Tasks { get; set; }
    }
}
