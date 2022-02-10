using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class RestrictionByDomainUpdateDTO
    {
        [Required]
        public string Domain { get; set; }
    }
}
