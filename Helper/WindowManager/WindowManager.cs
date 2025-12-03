using System.Windows;

namespace MedicalAir.Helper.WindowManager
{
    public static class WindowManager
    {
        public static void ShowAndCloseCurrent(Window newWindow)
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            
            newWindow.Show();
            
            if (currentWindow != null && currentWindow.IsLoaded && currentWindow != newWindow)
            {
                currentWindow.Close();
            }
        }
    }
}
