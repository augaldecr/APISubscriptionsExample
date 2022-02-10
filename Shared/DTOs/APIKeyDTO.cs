namespace Shared.DTOs
{
    public class APIKeyDTO
    {
        public string Key { get; set; }
        public string KeyType { get; set; }
        public bool Active { get; set; }
        public List<RestrictionByDomainDTO> RestrictionsByDomainDTOs { get; set; }
        public List<RestrictionByIPDTO> RestrictionsByIPDTOs { get; set; }
    }
}
