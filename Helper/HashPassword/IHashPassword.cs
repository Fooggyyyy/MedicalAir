namespace MedicalAir.Helper.HashPassword
{
    public interface IHashPassword
    {
        string HashPassword(string? password);
        bool Verify(string? PlainPassword, string? HashedPassword);
    }
}
