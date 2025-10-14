using CSMVCK8S.Shared.Entities;

namespace UserService.Model
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Photo>? Photos { get; set; }
    }
}