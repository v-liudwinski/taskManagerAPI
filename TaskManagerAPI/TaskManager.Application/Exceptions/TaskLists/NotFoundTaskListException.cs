namespace TaskManager.Application.Exceptions.TaskLists
{
    public class NotFoundTaskListException : Exception
    {
        public NotFoundTaskListException(string message) : base(message)
        {
        }
    }
}
