namespace IPservice_indigosoft.Models
{
    public class UserConnection
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public DateTime ConnectedAt { get; set; }
        public long UserId { get; set; }

        public User User { get; set; }
    }
}
