using System.Windows;
using System.Windows.Input;

namespace MedicalAir.Helper.Dialogs
{
    public partial class SimpleInputDialog : Window
    {
        public string Result { get; private set; }

        public SimpleInputDialog(string title, string message, string defaultValue = "")
        {
            InitializeComponent();
            Title = title;
            MessageTextBlock.Text = message;
            InputTextBox.Text = defaultValue;
            InputTextBox.SelectAll();
            InputTextBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = InputTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OkButton_Click(sender, new RoutedEventArgs());
            }
        }
    }
}






