using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RulesEngine.Web.Models;

namespace RulesEngine.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Demo()
        {
            Engine engine = new Engine();
            Employee employee = new Employee()
            {
                //FirstName = "Alan",
                LastName = "Wei",
                Age = 10
                //, Email = "abc"
            };
            engine.For<Employee>().Setup(e => e.FirstName).MustNotBeNullOrEmpty().WithMessage("{0}不能为空")
                .Setup(e => e.Email).WithMessage("邮箱格式不正确").MustMatchRegex(@"^\w+@\w+\.\w+$").MustNotBeNullOrEmpty();
            var report = new ValidationReport(engine);
            report.Validate(employee);
            var errors = report.GetErrorMessages(employee);

            return Json(new { errors }, JsonRequestBehavior.AllowGet);
        }
    }
}