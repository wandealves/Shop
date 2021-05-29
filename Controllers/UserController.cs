using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backoffice.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Services;

namespace Shop.Models
{
  [Route("v1/users")]
  public class UserController : ControllerBase
  {
    [HttpGet]
    [Route("")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
    {
      var users = await context.Users.AsNoTracking().ToListAsync();
      return users;
    }

    [HttpPost]
    [Route("")]
    [AllowAnonymous]
    public async Task<ActionResult<User>> Post([FromBody] User model, [FromServices] DataContext context)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      try
      {
        model.Role = "employee";

        context.Users.Add(model);
        await context.SaveChangesAsync();

        model.Password = "";
        return Ok(model);
      }
      catch
      {
        return BadRequest(new { message = "Não foi possível criar a usuário" });
      }
    }

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<User>> Put(
            [FromServices] DataContext context,
            int id,
            [FromBody] User model)
    {
      // Verifica se os dados são válidos
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      // Verifica se o ID informado é o mesmo do modelo
      if (id != model.Id)
        return NotFound(new { message = "Usuário não encontrada" });

      try
      {
        context.Entry(model).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return model;
      }
      catch (Exception)
      {
        return BadRequest(new { message = "Não foi possível criar o usuário" });

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

      user.Password = "";
      return new
      {
        user = user,
        token = token
      };
    }
  }
}