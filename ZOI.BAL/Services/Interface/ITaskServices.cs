using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZOI.Domain.Models;

namespace ZOI.BAL.Services.Interface
{
    public interface ITaskServices
    {
        public JsonResponse SaveTasks(Tasks data);
        public JsonResponse GetStatus();
        public JsonResponse GetTasks();
        public JsonResponse GetTasksById(int taskId);
    }
}
