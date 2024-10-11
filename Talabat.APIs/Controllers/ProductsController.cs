using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Repositories.contract;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.ProductSpacifications;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
    public class ProductsController : BaseApiController
    { 

        private readonly IMapper _mapper;
	    private readonly IUnitOfWork _unitOfWork;

	public ProductsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
		}


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            var spec = new ProductWithBrandAndCategorySpacifications(specParams);

            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            var mappedProducts = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductToReturnDto>>(products);

            var data = (IReadOnlyCollection<ProductToReturnDto>)mappedProducts;

            var countSpec = new ProductsWithFilterationForCountSpecification(specParams);

            int count = await _unitOfWork.Repository<Product>().GetCountAsync(countSpec);

            return Ok(new Pagination<ProductToReturnDto>(specParams.PageSize, specParams.PageIndex, count, data));
        }



        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var spec = new ProductWithBrandAndCategorySpacifications(id);

            var product = await _unitOfWork.Repository<Product>().GetWithSpecAsync(spec);

            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }

            var mappedProduct = _mapper.Map<Product, ProductToReturnDto>(product);
            return Ok(mappedProduct);
        }

        // Get Brands
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrand()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(brands);
        }

        // Get Categories
        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategory()
        {
            var categories = await _unitOfWork.Repository<ProductCategory>().GetAllAsync();
            return Ok(categories);
        }
    }
}
