using AutoMapper;
using BuildingBlocks.Core.Authorization;
using BuildingBlocks.Core.Responses;
using BuildingBlocks.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [CustomAuthorize(FeatureConstants.Product, PermissionLevel.Read)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _productRepository.GetAllAsync();
            if (!products.Any())
            {
                return NotFound("No products found in the database");
            }
            var response = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [CustomAuthorize(FeatureConstants.Product, PermissionLevel.Read)]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _productRepository.FindByIdAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} is not found");
            }
            var response = _mapper.Map<ProductDto>(product);
            return Ok(response);
        }

        [HttpPost]
        [CustomAuthorize(FeatureConstants.Product, PermissionLevel.Write)]
        public async Task<ActionResult<Response>> CreateProduct(ProductCreationDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = _mapper.Map<Product>(productDto);
            var response = await _productRepository.CreateAsync(product);
            return response.flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        [CustomAuthorize(FeatureConstants.Product, PermissionLevel.Write)]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = _mapper.Map<Product>(productDto);
            var response = await _productRepository.UpdateAsync(product);
            return response.flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id:int}")]
        [CustomAuthorize(FeatureConstants.Product, PermissionLevel.FullAccess)]
        public async Task<ActionResult<Response>> DeleteProduct(int id)
        {
            var response = await _productRepository.DeleteAsync(id);
            return response.flag ? Ok(response) : NotFound(response);
        }
    }
}
