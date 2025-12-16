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
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectServices _projectServices;

        public ProjectsController(IProjectServices projectServices)
        {
            _projectServices = projectServices;
        }

        [HttpPost(Name= "Projects")]
        public JsonResponse SaveProjects([FromBody] Projects data)
        {
            JsonResponse response = _projectServices.SaveProjects(data);
            return response;
        }

        [HttpGet(Name = "GetProjects")]
        public JsonResponse GetProjects()
        {
            JsonResponse response = _projectServices.GetProjects();
            return response;
        }

        [HttpGet(Name = "GetProjectsById")]
        public JsonResponse GetProjectsById(int projectId)
        {
            JsonResponse response = _projectServices.GetProjectsById(projectId);
            return response;
        }
    }
}
