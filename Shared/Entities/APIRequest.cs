namespace Shared.Entities
{
    public class APIRequest
    {
        public int Id { get; set; }
        public int KeyId { get; set; }
        public DateTime RequestDate { get; set; }
        public APIKey APIKey { get; set; }
    }
}
