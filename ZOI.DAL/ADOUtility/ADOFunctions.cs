using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZOI.DAL.Utilities.CommonFunctions;
using ZOI.Domain.Models;

namespace ZOI.DAL.ADOUtility
{
    public class ADOFunctions
    {
        private readonly string _connectionString;

        public ADOFunctions(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public JsonResponse GetDataSet(string procedureName, SqlParameter[]? sqlParameter = null)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {

                SqlConnection sqlConnection = new SqlConnection(_connectionString);
                SqlCommand sqlCommand = new SqlCommand(procedureName, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                if (sqlParameter != null)
                {
                    sqlCommand.Parameters.AddRange(sqlParameter);
                }
                DataSet dataSet = new DataSet();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dataSet);

                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Columns.Contains("Status") && dataSet.Tables[0].Columns.Contains("Message"))
                    {
                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            var row = dataSet.Tables[0].Rows[0];
                            jsonResponse.Status = row["Status"].ToString();
                            jsonResponse.Message = row["Message"].ToString();

                            if (dataSet.Tables[0].Columns.Contains("ID"))
                            {
                                jsonResponse.ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0;
                            }
                        }
                    }
                    else
                    {
                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            jsonResponse.Data = commonFunctions.DataTableToJson(dataSet.Tables[0]);
                        }
                        if (dataSet.Tables.Count > 0 && dataSet.Tables[1].Rows.Count > 0)
                        {
                            var row = dataSet.Tables[1].Rows[0];
                            jsonResponse.Status = row["Status"].ToString();
                            jsonResponse.Message = row["Message"].ToString();
                            if (dataSet.Tables[1].Columns.Contains("ID"))
                            {
                                jsonResponse.ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                jsonResponse.Status = "F";
                jsonResponse.Message = "Execution failed: " + ex.Message;
            }

            return jsonResponse;
        }
    }
}
