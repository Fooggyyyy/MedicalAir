namespace MedicalAir.Model.Entites
{
    public class Notification
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User? User { get; set; }

        public string? MessageBody { get; set; }
        public DateTime CreatedDate { get; set; }

        public Notification(int userId, string? messageBody)
        {
            UserId = userId;
            MessageBody = messageBody;
            CreatedDate = DateTime.Now;
        }

        public Notification() { }
    }
}
