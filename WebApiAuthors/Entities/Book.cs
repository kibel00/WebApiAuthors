using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Entities
{
    public class Book
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The {0} is requiered")]
        [FirstCapitalLetter]
        [StringLength(maximumLength: 220, ErrorMessage = "The field {0} should not have more than {1} character")]

        public string Title { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
