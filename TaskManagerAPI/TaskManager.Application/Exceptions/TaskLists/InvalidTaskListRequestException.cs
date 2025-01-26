namespace TaskManager.Application.Exceptions.TaskLists
{
    public class InvalidTaskListRequestException : Exception
    {
        public InvalidTaskListRequestException(string message) : base(message)
        {
        }
    }
}
