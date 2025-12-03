namespace MedicalAir.Helper.HashPassword
{
    public class HashPasswordService : IHashPassword
    {
        public string HashPassword(string? password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string? PlainPassword, string? HashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(PlainPassword, HashedPassword);
        }
    }
}
