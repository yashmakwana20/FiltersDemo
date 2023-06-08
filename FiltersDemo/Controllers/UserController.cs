using FiltersDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.OrmLite;

namespace FiltersDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly OrmLiteConnectionFactory dbFactory;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            dbFactory = new OrmLiteConnectionFactory(_configuration.GetConnectionString("con"), MySqlDialect.Provider);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult logIn(string username, string password)
        {
            using(var db = dbFactory.Open())
            {
                var users = db.Select<User>(x => x.userName == username && x.Password == password).FirstOrDefault();
                if(users == null)
                {
                    return NotFound();
                }

                TokenManager tm = new TokenManager(_configuration);
                string token = tm.tokenGenerator(users);
                return Ok(token);
            }
        }
    }
}
