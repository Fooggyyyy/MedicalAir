using System.Windows;
using System.Windows.Input;

namespace MedicalAir.Helper.Dialogs
{
    public partial class AddAirplaneDialog : Window
    {
        public string AirplaneName { get; private set; }

        public AddAirplaneDialog(string title = "Добавление самолета", string defaultValue = "")
        {
            InitializeComponent();
            Title = title;
            AirplaneNameTextBox.Text = defaultValue;
            AirplaneNameTextBox.SelectAll();
            AirplaneNameTextBox.Focus();
            AirplaneNameTextBox.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    OkButton_Click(s, new RoutedEventArgs());
                }
            };
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AirplaneNameTextBox.Text))
            {
                ModernMessageDialog.Show("Введите название самолета!", "Ошибка", MessageType.Warning);
                return;
            }

            AirplaneName = AirplaneNameTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
