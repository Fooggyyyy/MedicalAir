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
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }

}
