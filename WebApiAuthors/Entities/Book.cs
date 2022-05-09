using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Entities
{
    public class Book
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The {0} is requiered")]
        [FirstCapitalLetter]
        public string Title { get; set; }
        public Author Author { get; set; }
        public int AuthorId { get; set; }
    }
}
