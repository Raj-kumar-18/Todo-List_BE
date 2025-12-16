using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZOI.Domain.Models;

namespace ZOI.BAL.Services.Interface
{
    public interface IUserServices
    {
        public JsonResponse SignUp(Users data);
        public Task<JsonResponse> SocialLogin(TokenRequest data);
        public Task<JsonResponse> CustomLogin(Users data);
        public JsonResponse GetUsers();
    }
}
