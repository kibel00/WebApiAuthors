namespace WebApiAuthors.DTOs
{
    public class AuthorDTOWithBook : AuthorDTO
    {
        public List<BookDTO> Books { get; set; }
    }
}
