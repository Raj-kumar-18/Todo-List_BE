using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZOI.Domain.Models
{
    public class Tasks
    {
        public int Id { get; set; }
        public string? TaskTitle { get; set; }
        public int Projects { get; set; }
        public int Status {  get; set; }
        public int AssignTo { get; set; }
        public DateTime DueDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}
