using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.DTO;
using UserService.Model;
using MassTransit;
using CSMVCK8S.Shared.Events;


namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;
        private readonly IMapper _mapper;
        private readonly IBus _bus;

        public UserService(UserDbContext context, IMapper mapper, IBus bus)
        {
            _context = context;
            _mapper = mapper;
            _bus = bus;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> AddUserAsync(UserCreateDto createDto)
        {
            
            var userEntity = _mapper.Map<User>(createDto);
            
            
            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();
            
           
            var userDto = _mapper.Map<UserDto>(userEntity);

            await _bus.Publish<UserCreated>(new 
            {
                // MassTransit'te Guid kullanmak en iyisi olduğu için, 
                // burada bir Guid dönüşümü veya yeni bir Guid oluşturulması gerekebilir 
                // eğer DB Id'niz int ise. Geçici olarak int'i Guid'a çevirelim (TEST AMAÇLI)
                UserId = Guid.NewGuid(), // Normalde DB'den alınan tekil bir ID olmalı
                UserName = userEntity.FirstName, 
                Email = userEntity.Email,
                CreationDate = DateTime.UtcNow
            });

            // 4. Sonucu Döndürme
            return userDto;
        }
    }
}