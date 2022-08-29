using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.DTOs
{
    public class BookPatchDTO
    {
        [Required(ErrorMessage = "The {0} is requiered")]
        [FirstCapitalLetter]
        [StringLength(maximumLength: 220, ErrorMessage = "The field {0} should not have more than {1} character")]
        public string Title { get; set; }
        public DateTime? PublicationDate { get; set; }
    }
}
