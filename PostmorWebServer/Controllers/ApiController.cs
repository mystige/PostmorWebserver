using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PostmorWebServer.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private IUserService _userService;
        public ApiController(IUserService userService)
        {
            _userService = userService;
        }

       

        //[AllowAnonymous]
        [HttpGet("authenticate")]
        public IActionResult Authenticate([FromBody]User model)
        {
            var user = _userService.Authenticate(model.Email, model.PasswordHash);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
