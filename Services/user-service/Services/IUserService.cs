using UserService.DTO;

namespace UserService.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto> AddUserAsync(UserCreateDto createDto); 
    }
}