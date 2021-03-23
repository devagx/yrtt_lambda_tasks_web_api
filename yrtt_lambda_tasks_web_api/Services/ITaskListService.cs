using System.Collections;
using yrtt_lambda_tasks_web_api.Models;

namespace yrtt_lambda_tasks_web_api.Services
{
    public interface ITaskListService
    {
        ArrayList GetTasks(string taskId);
        ArrayList GetTasks();
        void AddTask(TaskListModel task);
        void DeleteTask(string taskId);
        void UpdateTask(TaskListModel task);
    }
}
