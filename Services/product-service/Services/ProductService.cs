using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.DTO;
using ProductService.Model;

namespace ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(ProductDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products.ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            return _mapper.Map<ProductDto>(product);
        }
        
        public async Task<ProductDto> AddProductAsync(ProductCreateDto createDto)
        {
            var productEntity = _mapper.Map<Product>(createDto);

            await _context.Products.AddAsync(productEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDto>(productEntity);
        }
    }
}