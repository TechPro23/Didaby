using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.API.Controllers
{

    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository productRepository, ILogger<CatalogController> logger)
        {
            _productRepository = productRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var allProd = await _productRepository.GetProducts();
            return Ok(allProd);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        public async Task<ActionResult<Product>> GetProducts(string id)
        {
            //Get a Product by Id
            var product = await _productRepository.GetProductById(id);
            if (product == null)
            {
                //log ot the erroe
                _logger.LogError($"Product with Id:{id}, not found");
                return NotFound();
            }
            return Ok(product);
        }

        [Route("[action]/{category}", Name = "GetProductByCAtegory")]
        [HttpGet]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            var categoryName = await _productRepository.GetProductByCategory(category);
            if (categoryName == null)
            {
                return NotFound();
                _logger.LogError($"Product with Category Name:{categoryName}, not found");
            }
            return Ok(categoryName);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _productRepository.CreateProduct(product);
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);

        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            var updated = await _productRepository.UpdateProduct(product);
            return NoContent();

        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> DeleteProduct(string id)
        {
            //Get a Product by Id
            var productId = await _productRepository.DeleteProduct(id);
            
            return Ok(productId);
        }
    }
}
