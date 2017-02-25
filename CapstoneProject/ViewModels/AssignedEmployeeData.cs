using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CapstoneProject.Models;

namespace CapstoneProject.ViewModels
{
    public class AssignedEmployeeData
    {
        public Employee Employee { get; set; }
        public bool Assigned { get; set; }
    }
}