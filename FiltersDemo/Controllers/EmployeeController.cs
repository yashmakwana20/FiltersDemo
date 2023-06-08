using FiltersDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.OrmLite;

namespace FiltersDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly OrmLiteConnectionFactory dbFactory;
        private readonly IConfiguration _config;

        public EmployeeController(IConfiguration config)
        {
            _config = config;
            dbFactory = new OrmLiteConnectionFactory(_config.GetConnectionString("con"), MySqlDialect.Provider);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult getAllData()
        {
            using (var db = dbFactory.Open())
            {
                var data = db.Select<Employee>();
                return Ok(data);
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("MaleData")]
        public IActionResult getMaleData()
        {
            using (var db = dbFactory.Open())
            {
                var getMale = db.Select<Employee>(x => x.Gender == "Male");
                if (getMale == null)
                {
                    return NotFound();
                }
                return Ok(getMale);
            }
        }

        [Authorize(Roles = "Admin,tester")]
        //[AllowAnonymous]
        [HttpPost("addData")]
        public IActionResult postData(Employee emp)
        {
            using (var db = dbFactory.Open())
            {
                db.Insert(new Employee { employeeName = emp.employeeName, employeeAge = emp.employeeAge, Gender = emp.Gender, Department = emp.Department, Salary = emp.Salary });

                return Ok("Data Inserted!!");
            }
        }
    }
}
