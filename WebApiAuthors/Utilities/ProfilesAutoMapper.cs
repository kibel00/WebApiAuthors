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
            CreateMap<CreationBookDTO, Book>();
            CreateMap<Book, BookDTO>();
            //Comments
            CreateMap<CommentCreationDTO, Comment>();
            CreateMap<Comment, CommentsDTO>();
        }
    }
}
