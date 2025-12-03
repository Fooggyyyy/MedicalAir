using System.Windows;
using MedicalAir.Model.Entites;

namespace MedicalAir.Helper.Dialogs
{
    public partial class EditCertificatDialog : Window
    {
        public DateTime? DataStart { get; private set; }
        public DateTime? DataEnd { get; private set; }

        public EditCertificatDialog(Certificat certificat)
        {
            InitializeComponent();
            
            if (certificat != null)
            {
                DataStartPicker.SelectedDate = certificat.DataStart.ToDateTime(TimeOnly.MinValue);
                DataEndPicker.SelectedDate = certificat.DataEnd.ToDateTime(TimeOnly.MinValue);
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DataStartPicker.SelectedDate.HasValue)
            {
                ModernMessageDialog.Show("Выберите дату начала", "Предупреждение", MessageType.Warning);
                return;
            }

            if (!DataEndPicker.SelectedDate.HasValue)
            {
                ModernMessageDialog.Show("Выберите дату окончания", "Предупреждение", MessageType.Warning);
                return;
            }

            if (DataStartPicker.SelectedDate.Value > DataEndPicker.SelectedDate.Value)
            {
                ModernMessageDialog.Show("Дата начала не может быть позже даты окончания", "Ошибка", MessageType.Error);
                return;
            }

            DataStart = DataStartPicker.SelectedDate.Value;
            DataEnd = DataEndPicker.SelectedDate.Value;

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
