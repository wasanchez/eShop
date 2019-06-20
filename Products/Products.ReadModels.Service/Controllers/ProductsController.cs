using System;
using MicroServices.Common.General.Exceptions;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Products.ReadModels.Service.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IActionResult Get()
        {
            var products = ServiceLocator.ProductView.GetAll();
            return Ok(products);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            try
            {
                var product = ServiceLocator.ProductView.GetById(id);
                return Ok(product);
            }
            catch (ReadModelNotFoundException)
            {
                return NotFound();
            }
        }
       
    }
}
