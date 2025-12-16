using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZOI.Domain.Models
{
    public class JsonResponse
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public int ID { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public object? Data { get; set; }
    }
}
