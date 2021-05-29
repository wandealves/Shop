using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backoffice.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Services;

namespace Shop.Models
{
  [Route("users")]
  public class UserController : ControllerBase
  {
    [HttpPost]
    [Route("")]
    public async Task<ActionResult<User>> Post([FromBody] User model, [FromServices] DataContext context)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      try
      {
        context.Users.Add(model);
        await context.SaveChangesAsync();
        return Ok(model);
      }
      catch
      {
        return BadRequest(new { message = "Não foi possível criar a usuário" });
      }
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<dynamic>> Authenticate([FromBody] User model, [FromServices] DataContext context)
    {
      var user = await context.Users.AsNoTracking().Where(x => x.Username == model.Username && x.Password == model.Password).FirstOrDefaultAsync();

      if (user == null)
        return NotFound(new { message = "Usuário ou senha inválido" });

      var token = TokenService.GenerateToken(user);

      return new
      {
        user = user,
        token = token
      };
    }
  }
}