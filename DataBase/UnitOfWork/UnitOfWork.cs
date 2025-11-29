using MedicalAir.DataBase;
using MedicalAir.DataBase.Repositories.Implementations;
using MedicalAir.DataBase.Repositories.Interfaces;
using System.Threading.Tasks;

namespace MedicalAir.DataBase.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DBContext _dbContext;

        public IAirplaneRepository AirplaneRepository { get; set; }
        public ICertificatRepository CertificatRepository { get; set; }
        public IHistoryUpMedicinRepository HistoryUpMedicinRepository { get; set; }
        public IMedicalExaminationRepository MedicalExaminationRepository { get; set; }
        public IMedicinRepository MedicinRepository { get; set; }
        public IMedkitRepository MedkitRepository { get; set; }
        public INotificationRepository NotificationRepository { get; set; }
        public IProcedureRepository ProcedureRepository { get; set; }
        public IRegistrationUserRepository RegistrationUserRepository { get; set; }
        public IReportMedicinRepository ReportMedicinRepository { get; set; }
        public IReportUserRepository ReportUserRepository { get; set; }
        public IUserProcedureRepository UserProcedureRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IUserRoleProcedureRepository RoleProcedureRepository { get; set; }

        public UnitOfWork(DBContext dbContext)
        {
            _dbContext = dbContext;

            AirplaneRepository = new AirplaneRepository(_dbContext);
            CertificatRepository = new CertificatRepository(_dbContext);
            HistoryUpMedicinRepository = new HistoryUpMedicinRepository(_dbContext);
            MedicalExaminationRepository = new MedicalExaminationRepository(_dbContext);
            MedicinRepository = new MedicinRepository(_dbContext);
            MedkitRepository = new MedkitRepository(_dbContext);
            NotificationRepository = new NotificationRepository(_dbContext);
            ProcedureRepository = new ProcedureRepository(_dbContext);
            RegistrationUserRepository = new RegistrationUserRepository(_dbContext);
            ReportMedicinRepository = new ReportMedicinRepository(_dbContext);
            ReportUserRepository = new ReportUserRepository(_dbContext);
            UserProcedureRepository = new UserProcedureRepository(_dbContext);
            UserRepository = new UserRepository(_dbContext);
            RoleProcedureRepository = new UserRoleProcedureRepository(_dbContext);
        }

        public UnitOfWork()
        {
        }

        public async Task<int> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
