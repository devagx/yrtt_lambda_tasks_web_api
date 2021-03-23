namespace yrtt_lambda_tasks_web_api.Models
{
    public class TaskListModel
    {
        public string TaskId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
    }
}
