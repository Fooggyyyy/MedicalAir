using MedicalAir.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MedicalAir.Config
{
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            using (var context = DbContextFactory.Create())
            {
                context.Database.EnsureCreated();
            }

            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }

    public static class DbContextFactory
    {
        public static DBContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DBContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MedicalAirDB;Trusted_Connection=True;");

            return new DBContext(optionsBuilder.Options);
        }
    }
}
