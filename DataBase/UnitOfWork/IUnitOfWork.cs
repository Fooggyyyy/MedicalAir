using MedicalAir.DataBase.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAir.DataBase.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAirplaneRepository AirplaneRepository { get; set; }
        ICertificatRepository CertificatRepository { get; set; }
        IHistoryUpMedicinRepository HistoryUpMedicinRepository { get; set; }
        IMedicalExaminationRepository MedicalExaminationRepository { get; set; }
        IMedicinRepository MedicinRepository { get; set; }
        IMedkitRepository MedkitRepository { get; set; }
        INotificationRepository NotificationRepository { get; set; }
        IProcedureRepository ProcedureRepository { get; set; }
        IRegistrationUserRepository RegistrationUserRepository { get; set; }
        IReportMedicinRepository ReportMedicinRepository { get; set; }
        IReportUserRepository ReportUserRepository { get; set; }
        IUserProcedureRepository UserProcedureRepository { get; set; }
        IUserRepository UserRepository { get; set; }
        IUserRoleProcedureRepository RoleProcedureRepository { get; set; }

        Task<int> SaveAsync();
    }
}
