using System.Collections;
using yrtt_lambda_tasks_web_api.Models;

namespace yrtt_lambda_tasks_web_api.Data
{
    public interface IDatabase
    {

        ArrayList GetTasks();
        ArrayList GetTasks(string taskId);
        void AddTask(TaskListModel task);
        void DeleteTask(string taskId);
        void UpdateTask(TaskListModel task);

    }
}
