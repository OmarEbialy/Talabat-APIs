using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{
    public class ProductsController : BaseAPIsController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // 1. 3awz aklm l IGenericRepository 3l4an atlop mno Product
        // 2. Allow DependancyInjection
        public ProductsController
            (
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //[Authorize]
        [Cached(600)] // Action Filter [Cached 10 Mins]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams SpecParams) // FromQuery 3l4an y run mn query
        {
            var spec = new ProductWithBrandAndTypeSpecifications(SpecParams);

            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            var countSpec = new ProductWithFilterationForCountSpecification(SpecParams);

            var count = await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(countSpec);

            return Ok(new Pagination<ProductToReturnDto>(SpecParams.PageIndex , SpecParams.PageSize ,count , data));
        }
       
        // Imoprove Swagger 
        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductWithBrandAndTypeSpecifications(id);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);

            // Error Checking
            if (product is null) return NotFound(new ApiResponse(404));

            return Ok(_mapper.Map<Product , ProductToReturnDto>(product));
        }

        [HttpGet("brands")]
        // GET : api/product/brands
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(brands);
        }

        [HttpGet("types")]
        // GET : api/product/types
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {
            var types = await _unitOfWork.Repository<ProductType>().GetAllAsync();
            return Ok(types);
        }
    }
}
