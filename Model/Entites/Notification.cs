using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAir.Model.Entites
{
    public class Notification
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User? User { get; set; }

        public string? MessageBody { get; set; }

        public Notification(int userId, string? messageBody)
        {
            UserId = userId;
            MessageBody = messageBody;
        }

        public Notification() { }
    }
}
