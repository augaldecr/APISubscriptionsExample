using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class RestrictionByIPUpdateDTO
    {
        [Required]
        public string IP { get; set; }
    }
}
