using Microsoft.AspNetCore.Mvc;
using UserService.Services;
using UserService.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Clients; // IProductClient için eklendi

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IProductClient _productClient;

        public UsersController(IUserService userService, IProductClient productClient)
        {
            _userService = userService;
            _productClient = productClient;
        }
        

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DTO.UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DTO.UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostUser([FromBody] UserCreateDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Data Annotations hatalarını yakalar
            }

            var newUser = await _userService.AddUserAsync(userDto);
            
            // 201 Created döndürür ve yeni kaynağın URI'ını belirtir.
            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
        }

        // Product Service'ten ürün detaylarını getiren yeni endpoint
        [HttpGet("ADS")]
        [ProducesResponseType(typeof(ProductClientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductDetailsFromProductService(int productId)
        {
            var product = await _productClient.GetProductDetails(productId);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}