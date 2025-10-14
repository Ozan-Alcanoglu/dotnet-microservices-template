using System.ComponentModel.DataAnnotations;

namespace UserService.DTO
{
    // Yeni kullanıcı oluşturmak için kullanılacak DTO
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        public string Password { get; set; } // Genellikle Auth servisinde Hashlenir. Burada basitleştiriyoruz.

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}