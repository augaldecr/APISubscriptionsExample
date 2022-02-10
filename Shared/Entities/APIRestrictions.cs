namespace Shared.Entities
{
    public abstract class APIRestrictions
    {
        public int Id { get; set; }
        public int KeyId { get; set; }
        public APIKey Key { get; set; }
    }
}
