using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZOI.BAL.Services.Interface;
using ZOI.DAL.ADOUtility;
using ZOI.DAL.Utilities.CommonFunctions;
using ZOI.Domain.Models;

namespace ZOI.BAL.Services
{
    public class ProjectServices: IProjectServices
    {
        private readonly ADOFunctions _aDOFunctions;

        public ProjectServices(ADOFunctions aDOFunctions)
        {
            _aDOFunctions = aDOFunctions;
        }

        public JsonResponse SaveProjects( Projects data)
        {
            SqlParameter[] sqlParameter = new SqlParameter[]
            {
                new SqlParameter(){ ParameterName = "@ProjectID", Value = data.ProjectId},
                new SqlParameter(){ ParameterName = "@Flag", Value = data.Flag},
                new SqlParameter(){ ParameterName = "@ProjectName", Value = data.ProjectName},
                new SqlParameter(){ ParameterName = "@ProjectDescription", Value = data.ProjectDescription},
                new SqlParameter(){ ParameterName = "@CreatedBy", Value = data.CreatedBy}
            };
            return _aDOFunctions.GetDataSet(Constants.SaveProjects, sqlParameter);
        }

        public JsonResponse GetProjects()
        {
            return _aDOFunctions.GetDataSet(Constants.GetProjects);
        }

        public JsonResponse GetProjectsById(int projectId)
        {
            SqlParameter[] sqlParameter = new SqlParameter[]
            {
                new SqlParameter(){ ParameterName = "@ProjectID", Value =  projectId},
            };
            return _aDOFunctions.GetDataSet(Constants.GetProjectsById, sqlParameter);
        }
    }
}
