using System.Windows;
using MedicalAir.Model.Entites;

namespace MedicalAir.Helper.Dialogs
{
    public partial class EditMedicineDialog : Window
    {
        public string MedicineName { get; private set; }
        public int Count { get; private set; }
        public DateTime? UpData { get; private set; }
        public DateTime? EndData { get; private set; }

        public EditMedicineDialog(HistoryUpMedicin medicine)
        {
            InitializeComponent();
            
            if (medicine != null)
            {
                NameTextBox.Text = medicine.Name;
                CountTextBox.Text = medicine.Count.ToString();
                UpDataPicker.SelectedDate = medicine.UpData.ToDateTime(TimeOnly.MinValue);
                EndDataPicker.SelectedDate = medicine.EndData.ToDateTime(TimeOnly.MinValue);
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ModernMessageDialog.Show("Введите название лекарства", "Предупреждение", MessageType.Warning);
                return;
            }

            if (!int.TryParse(CountTextBox.Text, out int count) || count <= 0)
            {
                ModernMessageDialog.Show("Введите корректное количество (больше 0)", "Ошибка", MessageType.Error);
                return;
            }

            if (!UpDataPicker.SelectedDate.HasValue)
            {
                ModernMessageDialog.Show("Выберите дату пополнения", "Предупреждение", MessageType.Warning);
                return;
            }

            if (!EndDataPicker.SelectedDate.HasValue)
            {
                ModernMessageDialog.Show("Выберите дату окончания срока", "Предупреждение", MessageType.Warning);
                return;
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var upDate = DateOnly.FromDateTime(UpDataPicker.SelectedDate.Value.Date);
            var maxUpDate = today.AddDays(7);
            
            if (upDate > maxUpDate)
            {
                ModernMessageDialog.Show($"Дата добавления не может быть позже {maxUpDate:dd.MM.yyyy} (7 дней от сегодня)", "Ошибка", MessageType.Error);
                return;
            }

            MedicineName = NameTextBox.Text;
            Count = count;
            UpData = UpDataPicker.SelectedDate.Value;
            EndData = EndDataPicker.SelectedDate.Value;

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
