using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RulesEngine.Web.Models
{
    public class Employee : Person
    {
        public decimal Salary { get; set; }
        public DateTime SalaryDate { get; set; }
    }
}