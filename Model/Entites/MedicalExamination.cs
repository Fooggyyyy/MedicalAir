namespace MedicalAir.Model.Entites
{
    public class MedicalExamination
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int URPId { get; set; }
        public UserRoleProcedure? UserRoleProcedure { get; set; } 

        public DateOnly DataStart { get; set; }
        public DateOnly DataEnd { get; set; }
        public bool IsValid { get; set; }
        public string? Message { get; set; }

        public MedicalExamination(int userId, int urpId, DateOnly dataStart, DateOnly dataEnd, bool isValid, string? message)
        {
            UserId = userId;
            DataStart = dataStart;
            DataEnd = dataEnd;
            IsValid = isValid;
            Message = message;
            URPId = urpId;
        }

        public MedicalExamination() { }
    }
}
