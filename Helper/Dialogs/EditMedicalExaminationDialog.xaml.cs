using System;
using System.Windows;
using MedicalAir.Helper.Dialogs;
using MedicalAir.Model.Entites;

namespace MedicalAir.Helper.Dialogs
{
    public partial class EditMedicalExaminationDialog : Window
    {
        public DateTime? DataStart { get; private set; }
        public DateTime? DataEnd { get; private set; }
        public string? Message { get; private set; }

        public EditMedicalExaminationDialog(MedicalExamination examination)
        {
            InitializeComponent();
            
            if (examination != null)
            {
                DataStartPicker.SelectedDate = examination.DataStart.ToDateTime(TimeOnly.MinValue);
                DataEndPicker.SelectedDate = examination.DataEnd.ToDateTime(TimeOnly.MinValue);
                MessageTextBox.Text = examination.Message ?? "";
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
            Message = string.IsNullOrWhiteSpace(MessageTextBox.Text) ? null : MessageTextBox.Text.Trim();

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
