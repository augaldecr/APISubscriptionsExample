using Microsoft.AspNetCore.Identity;

namespace Shared.Entities
{
    public class APIKey
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public KeyType KeyType { get; set; }
        public bool Active { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<RestrictionByDomain> RestrictionsByDomain { get; set; }
        public List<RestrictionByIP> RestrictionsByIP { get; set; }
    }

    public enum KeyType
    {
        Free = 1,
        Professional = 2,
    }
}
