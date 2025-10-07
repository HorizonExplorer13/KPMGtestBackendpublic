using Microsoft.AspNetCore.Mvc;
using testKPMG.DTOs.Products;
using testKPMG.Interfaces;

namespace testKPMG.Controllers.Product
{
    [ApiController]
    [Route("Api/[Controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService service;

        public ProductController(IProductService service)
        {
            this.service = service;
        }
        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> GetList()
        {
            var response = await service.GetProductsList();
            return Ok(response);
        }
        [HttpGet]
        [Route("ById/{Id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var response = await service.GetProductById(Id);
            return Ok(response);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] PostProductDTO postProduct)
        {
            await service.CreateProduct(postProduct);
            return Ok();
        }
        [HttpPut]
        [Route("Update/{Id}")]
        public async Task<IActionResult> Update(Guid Id, [FromBody] PostProductDTO updateProduct)
        {
            await service.UpdateProduct(Id, updateProduct);
            return Ok();
        }
        [HttpDelete]
        [Route("Delete/{Id}")]
        public async Task<IActionResult> GetList(Guid Id)
        {
            await service.DeleteProduct(Id);
            return Ok();
        }


    }
}
