namespace Shared.Entities
{
    public class Bill
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public bool Paid { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime PaydayLimit { get; set; }
    }
}
