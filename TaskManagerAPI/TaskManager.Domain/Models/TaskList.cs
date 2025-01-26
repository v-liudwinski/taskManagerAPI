namespace TaskManager.Domain.Models
{
    public class TaskList
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string OwnerId { get; set; }

        public List<UserTaskListRelation> UserRelations { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
