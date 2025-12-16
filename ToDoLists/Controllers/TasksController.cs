using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZOI.BAL.Services.Interface;
using ZOI.Domain.Models;

namespace ToDoList.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskServices _taskServices;

        public TasksController(ITaskServices taskServices)
        {
            _taskServices = taskServices;
        }

        [HttpPost(Name = "Tasks")]
        public JsonResponse SaveTasks([FromBody] Tasks data)
        {
            JsonResponse response = _taskServices.SaveTasks(data);
            return response;
        }

        [HttpGet(Name = "Get Status")]
        public JsonResponse GetStatus()
        {
            JsonResponse response = _taskServices.GetStatus();
            return response;
        }

        [HttpGet(Name = "GetTasks")]
        public JsonResponse GetTasks()
        {
           JsonResponse response = _taskServices.GetTasks();
            return response;
        }

        [HttpGet(Name = "GetTasksById")]
        public JsonResponse GetTasksById(int taskId)
        {
            JsonResponse response = _taskServices.GetTasksById(taskId);
            return response;
        }
    }
}
