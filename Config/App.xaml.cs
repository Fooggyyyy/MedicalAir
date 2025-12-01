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
                
                // Добавляем колонку IsBlocked, если её нет
                try
                {
                    var connection = context.Database.GetDbConnection();
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        // Проверяем, существует ли колонка
                        command.CommandText = @"
                            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'IsBlocked')
                            BEGIN
                                ALTER TABLE [Users] ADD [IsBlocked] bit NOT NULL DEFAULT 0;
                            END";
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                catch
                {
                    // Игнорируем ошибки при добавлении колонки
                }

                // Добавляем колонку CreatedDate в Notifications, если её нет
                try
                {
                    var connection = context.Database.GetDbConnection();
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Notifications]') AND name = 'CreatedDate')
                            BEGIN
                                ALTER TABLE [Notifications] ADD [CreatedDate] datetime2 NOT NULL DEFAULT GETDATE();
                            END";
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                catch
                {
                    // Игнорируем ошибки при добавлении колонки
                }

                // Очищаем уведомления старше недели
                try
                {
                    var connection = context.Database.GetDbConnection();
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            DELETE FROM [Notifications] 
                            WHERE [CreatedDate] < DATEADD(day, -7, GETDATE())";
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                catch
                {
                    // Игнорируем ошибки при очистке
                }
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
