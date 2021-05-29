using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backoffice.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shop.Models
{
  [Route("products")]
  public class ProductController : ControllerBase
  {
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
    {
      var products = await context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();
      return Ok(products);
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id, [FromServices] DataContext context)
    {
      var product = await context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

      return Ok(product);
    }

    [HttpGet]
    [Route("categories/{id:int}")]
    public async Task<ActionResult<List<Product>>> GetByCategory(int id, [FromServices] DataContext context)
    {
      var products = await context.Products.Include(x => x.Category).AsNoTracking().Where(x => x.CategoryId == id).ToListAsync();

      return Ok(products);
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<Product>> Post([FromBody] Product model, [FromServices] DataContext context)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      try
      {
        context.Products.Add(model);
        await context.SaveChangesAsync();
        return Ok(model);
      }
      catch
      {
        return BadRequest(new { message = "Não foi possível criar a produto" });
      }
    }

    /*[HttpPut]
    [Route("{id:int}")]
    public async Task<ActionResult<Category>> Put(int id, [FromBody] Category model, [FromServices] DataContext context)
    {
      if (model.Id != id)
        return NotFound(new { message = "Categoria não encontrada" });

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      try
      {
        context.Entry<Category>(model).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return Ok(model);
      }
      catch (DbUpdateConcurrencyException)
      {
        return BadRequest(new { message = "Este registro já foi atualizado" });
      }
      catch (Exception)
      {
        return BadRequest(new { message = "Não foi possível criar a categoria" });
      }
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ActionResult<Category>> Delete(int id, [FromServices] DataContext context)
    {
      var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

      if (category == null)
        return NotFound(new { message = "Categoria não encontrada" });

      try
      {
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return Ok(new { message = "Categoria removida com sucesso!" });
      }
      catch (Exception)
      {
        return BadRequest(new { message = "Não foi possível remover a categoria" });
      }

    }*/
  }
}