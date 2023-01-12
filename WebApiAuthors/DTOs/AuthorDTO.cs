using WebApiAuthors.Entities;

namespace WebApiAuthors.DTOs
{
    public class AuthorDTO: Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
