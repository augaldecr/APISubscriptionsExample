﻿namespace Shared.DTOs
{
    public class BookDTOWithAuthors : BookDTO
    {
        public List<AuthorDTO> Authors { get; set; }
    }
}
