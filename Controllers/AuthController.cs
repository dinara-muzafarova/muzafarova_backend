using Azure;
using Microsoft.AspNetCore.Mvc;
using muzafarova_backend.Models;

namespace muzafarova_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public struct LoginData
        {
            public string login { get; set; }
            public string password { get; set; }
        }
        [HttpPost]
        public object GetToken([FromBody] LoginData ld)
        {
            var user = SharedData.users.FirstOrDefault(u => u.Login == ld.login && u.Password == ld.password);
            if (user == null)
            {
                Response.StatusCode = 401;
                return new { message = "wrong login/password" };
            }
            return AuthOptions.GenerateToken(user.IsAdmin);
        }
        [HttpGet("users")]
        public List<user> Getusers()
        {
            return SharedData.users;
        }
        [HttpGet("token")]
        public object GetToken()
        {
            return AuthOptions.GenerateToken();
        }
        [HttpGet("token/secret")]
        public object GetAdminToken()
        {
            return AuthOptions.GenerateToken(true);
        }
    }
}
