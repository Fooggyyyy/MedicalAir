using System.Linq;
using System.Windows;

namespace MedicalAir.Helper.WindowManager
{
    public static class WindowManager
    {
        /// <summary>
        /// Закрывает все окна приложения, кроме указанного типа
        /// </summary>
        public static void CloseAllWindowsExcept<T>() where T : Window
        {
            var windowsToClose = Application.Current.Windows.OfType<Window>()
                .Where(w => !(w is T))
                .ToList();

            foreach (var window in windowsToClose)
            {
                window.Close();
            }
        }

        /// <summary>
        /// Закрывает все окна указанного типа
        /// </summary>
        public static void CloseAllWindowsOfType<T>() where T : Window
        {
            var windowsToClose = Application.Current.Windows.OfType<T>().ToList();

            foreach (var window in windowsToClose)
            {
                window.Close();
            }
        }

        /// <summary>
        /// Закрывает все окна приложения
        /// </summary>
        public static void CloseAllWindows()
        {
            var windowsToClose = Application.Current.Windows.OfType<Window>().ToList();

            foreach (var window in windowsToClose)
            {
                window.Close();
            }
        }

        /// <summary>
        /// Открывает новое окно и закрывает все остальные окна приложения
        /// </summary>
        public static void ShowWindowAndCloseOthers<T>(T newWindow) where T : Window
        {
            CloseAllWindows();
            newWindow.Show();
        }

        /// <summary>
        /// Открывает новое окно и закрывает все окна указанного типа
        /// </summary>
        public static void ShowWindowAndCloseType<TNew, TClose>(TNew newWindow) 
            where TNew : Window 
            where TClose : Window
        {
            CloseAllWindowsOfType<TClose>();
            newWindow.Show();
        }

        /// <summary>
        /// Открывает новое окно и закрывает текущее активное окно
        /// </summary>
        public static void ShowAndCloseCurrent(Window newWindow)
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            // Сначала показываем новое окно
            newWindow.Show();
            // Затем закрываем старое, если оно еще загружено
            if (currentWindow != null && currentWindow.IsLoaded && currentWindow != newWindow)
            {
                currentWindow.Close();
            }
        }

        /// <summary>
        /// Открывает новое окно как диалог и закрывает текущее активное окно
        /// </summary>
        public static void ShowDialogAndCloseCurrent(Window newWindow)
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            newWindow.ShowDialog();
            currentWindow?.Close();
        }
    }
}

