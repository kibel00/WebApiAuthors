using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.DTOs
{
    public class EditAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
