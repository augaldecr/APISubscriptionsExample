﻿namespace Shared.DTOs
{
    public class AuthorDTOWithBooks : AuthorDTO
    {
        public List<BookDTO> Books { get; set; }
    }
}
