using FirstAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;
using FirstAPI.Domain.Model;
using FirstAPI.Domain.Model.CarroAggregate;

namespace FirstAPI.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : Controller
    {
        [HttpPost]
        public IActionResult Auth(String username, String password)
        {
            if (username == "gabriel" || password == "123456")
            {
                var token = TokenService.GenerateToken(new Carro());
                return Ok(token);
            }
            return BadRequest("Username or password invalid");
        }
    }
}
