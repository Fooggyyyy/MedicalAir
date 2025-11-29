using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAir.Helper.HashPassword
{
    public interface IHashPassword
    {
        string HashPassword(string? password);
        bool Verify(string? PlainPassword, string? HashedPassword);
    }
}
