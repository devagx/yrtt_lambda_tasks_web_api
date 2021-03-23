using Microsoft.AspNetCore.Mvc;
using System;
using yrtt_lambda_tasks_web_api.Logging;
using yrtt_lambda_tasks_web_api.Models;
using yrtt_lambda_tasks_web_api.Services;

namespace yrtt_lambda_tasks_web_api.Controllers
{
    [Route("v1/taskList")]
    public class TaskListController : Controller
    {
        private readonly ITaskListService taskListService;

        public TaskListController(ITaskListService taskListService)
        {
            Logger.LogDebug("Setting taskListService", "TaskListController", "TaskListController");
            this.taskListService = taskListService;
        }
        [HttpGet("{taskId}")]
        public IActionResult GetSingleTask(string taskId)
        {
            try
            {
                Logger.LogDebug("Calling GetTasks", "GetSingleTask", "TaskListController");
                var result = taskListService.GetTasks(taskId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        [HttpGet]
        public IActionResult GetAllTasks()
        {
            try
            {
                Logger.LogDebug("Calling GetTasks", "GetAllTasks", "TaskListController");
                var result = taskListService.GetTasks();
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        [HttpPost]
        public IActionResult AddTask([FromBody]TaskListModel taskList)
        {
            try
            {
                Logger.LogDebug("Calling AddTask", "AddTask", "TaskListController");
                taskListService.AddTask(taskList);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        [HttpDelete]
        public IActionResult DeleteTask([FromBody]TaskListModel taskList)
        {
            try
            {
                Logger.LogDebug("Calling DeleteTask", "DeleteTask", "TaskListController");
                taskListService.DeleteTask(taskList.TaskId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        [HttpPut]
        public IActionResult UpdateTask([FromBody]TaskListModel taskList)
        {
            try
            {
                Logger.LogDebug("Calling UpdateTask", "UpdateTask", "TaskListController");
                taskListService.UpdateTask(taskList);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}