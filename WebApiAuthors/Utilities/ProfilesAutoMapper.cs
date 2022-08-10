﻿using AutoMapper;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Utilities
{
    public class ProfilesAutoMapper : Profile
    {
        public ProfilesAutoMapper()
        {
            CreateMap<CreationAuthorDTO, Author>();
            CreateMap<Author, AuthorDTO>();
            //Books
            CreateMap<CreationBookDTO, Book>().ForMember(book => book.AuthorsBooks, options => options.MapFrom(MapAuthorsBooks));
            CreateMap<Book, BookDTO>()
                .ForMember(bookDTO => bookDTO.Authors, options => options.MapFrom(MapBookDTOAuthors));
            //Comments
            CreateMap<CommentCreationDTO, Comment>();
            CreateMap<Comment, CommentsDTO>();
        }

        private List<AuthorDTO> MapBookDTOAuthors(Book book,BookDTO bookDTO)
        {
            var results = new List<AuthorDTO>();
            if (book.AuthorsBooks == null)
            {
                return results;
            }

            foreach (var item in book.AuthorsBooks)
            {
                results.Add(new AuthorDTO()
                {
                    Id = item.AuthorId,
                    Name = item.Author.Name
                });
            }
            return results;
        }
        private List<AuthorBook> MapAuthorsBooks(CreationBookDTO creationBookDTO, Book book)
        {
            var result = new List<AuthorBook>();
            if (creationBookDTO.AuthorsId == null)
            {
                return result;
            }

            foreach (var item in creationBookDTO.AuthorsId)
            {
                result.Add(new AuthorBook() { AuthorId = item });
            }
            return result;
        }
    }
}
