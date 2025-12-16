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
    public class TaskServices : ITaskServices
    {
        private readonly ADOFunctions _aDOFunctions;
        public TaskServices(ADOFunctions aDOFunctions)
        {
            _aDOFunctions = aDOFunctions;
        }

        public JsonResponse SaveTasks( Tasks data)
        {
            SqlParameter[] sqlParameter = new SqlParameter[]
            {
                new SqlParameter(){ ParameterName = "@TasksID", Value = data.Id},
                new SqlParameter(){ ParameterName = "@TaskTitle", Value = data.TaskTitle},
                new SqlParameter(){ ParameterName = "@ProjectID", Value = data.Projects},
                new SqlParameter(){ ParameterName = "@StatusID", Value = data.Status},
                new SqlParameter(){ ParameterName = "@UserID", Value = data.AssignTo},
                new SqlParameter(){ ParameterName = "@DueDate", Value = data.DueDate},
                new SqlParameter(){ ParameterName = "@CreatedBy", Value = data.CreatedBy},
            };
            return _aDOFunctions.GetDataSet(Constants.SaveTasks, sqlParameter);
        }

        public JsonResponse GetStatus()
        {
            return _aDOFunctions.GetDataSet(Constants.GetStatus);
        }

        public JsonResponse GetTasks()
        {
            return _aDOFunctions.GetDataSet(Constants.GetTasks);
        }

        public JsonResponse GetTasksById(int taskId)
        {
            SqlParameter[] sqlParameter = new SqlParameter[]
            {
                new SqlParameter(){ ParameterName = "@TasksID", Value =  taskId},
            };
            return _aDOFunctions.GetDataSet(Constants.GetTasksById, sqlParameter);
        }
    }
}
