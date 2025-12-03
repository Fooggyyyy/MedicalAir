namespace MedicalAir.Model.Entites
{
    public class RegistrationUser
    {
        public int Id { get; set; }

        public int AirplaneId { get; set; }
        public Airplane? Airplane { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public bool IsRegister { get; set; }
        public string? MessageBody { get; set; }
        public DateOnly Data { get; set; }

        public RegistrationUser(int airplaneId, int userId, bool isRegister, string? messageBody, DateOnly data)
        {
            AirplaneId = airplaneId;
            UserId = userId;
            IsRegister = isRegister;
            MessageBody = messageBody;
            Data = data;
        }

        public RegistrationUser() { }
    }
}
