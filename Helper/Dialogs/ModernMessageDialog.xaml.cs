using System.Windows;

namespace MedicalAir.Helper.Dialogs
{
    public enum MessageType
    {
        Info,
        Success,
        Warning,
        Error,
        Question
    }

    public partial class ModernMessageDialog : Window
    {
        public bool? Result { get; private set; }

        public ModernMessageDialog(string message, string title = "Уведомление", MessageType type = MessageType.Info)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
            TitleTextBlock.Text = title;

            // Настройка в зависимости от типа сообщения
            switch (type)
            {
                case MessageType.Success:
                    IconTextBlock.Text = "✓";
                    IconTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                    TitleTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                    break;
                case MessageType.Warning:
                    IconTextBlock.Text = "⚠";
                    IconTextBlock.Foreground = System.Windows.Media.Brushes.Orange;
                    TitleTextBlock.Foreground = System.Windows.Media.Brushes.Orange;
                    break;
                case MessageType.Error:
                    IconTextBlock.Text = "✕";
                    IconTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                    TitleTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                    break;
                case MessageType.Question:
                    IconTextBlock.Text = "?";
                    IconTextBlock.Foreground = System.Windows.Media.Brushes.Blue;
                    TitleTextBlock.Foreground = System.Windows.Media.Brushes.Blue;
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    OkButton.Visibility = Visibility.Collapsed;
                    break;
                default: // Info
                    IconTextBlock.Text = "ℹ";
                    IconTextBlock.Foreground = System.Windows.Media.Brushes.Blue;
                    TitleTextBlock.Foreground = System.Windows.Media.Brushes.Blue;
                    break;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            DialogResult = true;
            Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            DialogResult = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            DialogResult = false;
            Close();
        }

        public static void Show(string message, string title = "Уведомление", MessageType type = MessageType.Info)
        {
            var dialog = new ModernMessageDialog(message, title, type);
            dialog.ShowDialog();
        }

        public static bool? ShowQuestion(string message, string title = "Вопрос")
        {
            var dialog = new ModernMessageDialog(message, title, MessageType.Question);
            dialog.ShowDialog();
            return dialog.Result;
        }
    }
}

