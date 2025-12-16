using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZOI.Domain.Models;

namespace ZOI.BAL.Services.Interface
{
    public  interface IProjectServices
    {
        public JsonResponse SaveProjects( Projects data);
        public JsonResponse GetProjects();
        public JsonResponse GetProjectsById(int projectId);
    }
}
