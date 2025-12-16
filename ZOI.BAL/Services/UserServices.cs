using FirebaseAdmin.Auth;
using FirebaseAdmin.Auth.Hash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ZOI.BAL.Services.Interface;
using ZOI.DAL.ADOUtility;
using ZOI.DAL.Utilities.CommonFunctions;
using ZOI.Domain.Models;

namespace ZOI.BAL.Services
{
    public class UserServices : IUserServices
    {
        private readonly ADOFunctions _aDOFunctions;
        private readonly commonFunctions _commonFunctions;

        public UserServices(ADOFunctions aDOFunctions, IConfiguration configuration)
        {
            _aDOFunctions = aDOFunctions;
            _commonFunctions = new commonFunctions(configuration);  
        }
        public JsonResponse SignUp( Users data)
        {
            string ?passwordHash = null;
            if(data.Provider == "Custom")
            {
                passwordHash = BCrypt.Net.BCrypt.HashPassword(data.Password);
            }

            SqlParameter[] sqlParameter = new SqlParameter[]
            {
                new SqlParameter(){ ParameterName = "@ID", Value = data.Id},
                new SqlParameter(){ ParameterName = "@FullName", Value = data.FullName},
                new SqlParameter(){ ParameterName = "@Email", Value = data.Email},
                new SqlParameter(){ ParameterName = "@PasswordHash", Value = passwordHash},
                new SqlParameter(){ ParameterName = "@Role", Value = data.Role},
                new SqlParameter(){ ParameterName = "@Provider", Value = data.Provider},
            };
            var response = _aDOFunctions.GetDataSet(Constants.SaveUsers, sqlParameter);

            // Jwt & RefreshToken
            if (response.Status == "S")
            {
                response.Token = _commonFunctions.GenerateJwtToken(
                    response.ID.ToString(),
                    data.Email,
                    data.Role ?? "Junior",
                    data.FullName
                    );
                response.RefreshToken = _commonFunctions.GenerateRefreshToken(
                    response.ID.ToString(),
                    data.Email,
                    data.Role ?? "Junior",
                    data.FullName
                    );
            }
            return response;
        }

        public async Task<JsonResponse> SocialLogin( TokenRequest data)
        {
            JsonResponse response = new JsonResponse();
            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(data.Token);
                string email = decodedToken.Claims["email"].ToString();
                string name = decodedToken.Claims.ContainsKey("name") ? decodedToken.Claims["name"].ToString() : email;
                string provider = "Firebase";
                string role = "Junior";

                SqlParameter[] sqlParameter = new SqlParameter[]
                {
                    new SqlParameter(){ ParameterName = "@FullName", Value = name},
                    new SqlParameter(){ ParameterName = "@Email", Value = email},
                    new SqlParameter(){ ParameterName = "@Role", Value = role},
                    new SqlParameter(){ ParameterName = "@Provider", Value = provider},
                };
                response = _aDOFunctions.GetDataSet(Constants.SaveUsers, sqlParameter);

               //Jwt & RefreshToken
                if (response.Status == "S")
                {
                    response.Token = _commonFunctions.GenerateJwtToken(response.ID.ToString(), email, role, name);
                    response.RefreshToken = _commonFunctions.GenerateRefreshToken(response.ID.ToString(), email, role, name);
                }
            }
            catch(Exception ex)
            {
                response.Status = "F";
                response.Message = "Social Login Failed:" + ex.Message;
            }
            return response;
        }

        public async Task<JsonResponse> CustomLogin( Users data)
        {
            JsonResponse response = new JsonResponse();
            try
            {
                SqlParameter[] sqlParameter = new SqlParameter[]
                {
                    new SqlParameter(){ ParameterName = "@Email", Value = data.Email},
                    new SqlParameter(){ ParameterName = "@Password", Value = data.Password},            
                };
                response = _aDOFunctions.GetDataSet(Constants.CustomLogin, sqlParameter);
                if(response == null || response.Status == "F")
                {
                    return response;
                }
                var dataList = response.Data as List<Dictionary<string, object>>;
                if(dataList == null || dataList.Count == 0)
                {
                    response.Status = "F";
                    response.Message = "Invalid login credentials.";
                    return response;
                }

                var row = dataList.First();
                string? storedHash = row.ContainsKey("PasswordHash") ? row["PasswordHash"].ToString() : null;
                string? role = row.ContainsKey("Role") ? row["Role"].ToString() : "Junior";
                string? email = row.ContainsKey("Email") ? row["Email"].ToString() : data.Email;
                string? fullName = row.ContainsKey("FullName") ? row["FullName"].ToString() : "";
                int id = row.ContainsKey("UserID") ? Convert.ToInt32(row["UserID"]) : 0;

                // Verify Password
                bool isValid = BCrypt.Net.BCrypt.Verify(data.Password, storedHash);
                if (string.IsNullOrEmpty(storedHash) || !isValid)
                {
                    response.Status = "F";
                    response.Message = "Invalid password.";
                    return response;
                }

                // Jwt + Refresh Tokens
                response.Token = _commonFunctions.GenerateJwtToken(id.ToString(), email, role, fullName);
                response.RefreshToken = _commonFunctions.GenerateRefreshToken(id.ToString(), email, role, fullName);
            }
            catch (Exception ex)
            {
                response.Status = "F";
                response.Message = "Login Failed:" + ex.Message;
            }
            return response;
        }

       
        public JsonResponse GetUsers()
        {
            return _aDOFunctions.GetDataSet(Constants.GetUsers);
        }

    }
}
