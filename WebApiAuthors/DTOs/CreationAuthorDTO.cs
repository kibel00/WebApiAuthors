using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.DTOs
{
    public class CreationAuthorDTO
    {
        [Required(ErrorMessage = "Este campo es requerido")]
        [StringLength(maximumLength: 120, ErrorMessage = "The field {0} should not have more than {1} character")]
        [FirstCapitalLetter]
        public string Name { get; set; }
    }
}
