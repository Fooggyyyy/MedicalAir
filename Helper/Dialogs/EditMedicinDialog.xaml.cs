using System.Windows;
using MedicalAir.Model.Entites;

namespace MedicalAir.Helper.Dialogs
{
    public partial class EditMedicinDialog : Window
    {
        public string MedicineName { get; private set; }
        public string MedicineComposition { get; private set; }

        public EditMedicinDialog(Medicin medicin)
        {
            InitializeComponent();
            
            if (medicin != null)
            {
                NameTextBox.Text = medicin.Name ?? "";
                CompositionTextBox.Text = medicin.Composition ?? "";
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ModernMessageDialog.Show("Введите название лекарства", "Предупреждение", MessageType.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(CompositionTextBox.Text))
            {
                ModernMessageDialog.Show("Введите состав лекарства", "Предупреждение", MessageType.Warning);
                return;
            }

            MedicineName = NameTextBox.Text.Trim();
            MedicineComposition = CompositionTextBox.Text.Trim();

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
