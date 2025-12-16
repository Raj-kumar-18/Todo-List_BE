using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZOI.Domain.Models
{
    public class Projects
    {
        public int ProjectId { get; set; }
        public bool? Flag {  get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public string? CreatedBy { get; set; }
    }
}
