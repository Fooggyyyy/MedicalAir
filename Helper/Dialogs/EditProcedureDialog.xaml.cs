using System.Windows;
using MedicalAir.Model.Entites;

namespace MedicalAir.Helper.Dialogs
{
    public partial class EditProcedureDialog : Window
    {
        public string ProcedureName { get; private set; }
        public string ProcedureDescription { get; private set; }
        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }
        public string Units { get; private set; }
        public bool MustBeTrue { get; private set; }

        public EditProcedureDialog(Procedure procedure)
        {
            InitializeComponent();
            
            if (procedure != null)
            {
                NameTextBox.Text = procedure.Name;
                DescriptionTextBox.Text = procedure.Description;
                MinValueTextBox.Text = procedure.MinValue.ToString();
                MaxValueTextBox.Text = procedure.MaxValue.ToString();
                UnitsTextBox.Text = procedure.Units;
                MustBeTrueCheckBox.IsChecked = procedure.MustBeTrue;
                
                if (procedure.MustBeTrue)
                {
                    MinValueTextBox.IsEnabled = false;
                    MaxValueTextBox.IsEnabled = false;
                    UnitsTextBox.IsEnabled = false;
                }
            }
        }

        private void MustBeTrueCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MinValueTextBox.Text = "0";
            MaxValueTextBox.Text = "1";
            MinValueTextBox.IsEnabled = false;
            MaxValueTextBox.IsEnabled = false;
            UnitsTextBox.IsEnabled = false;
        }

        private void MustBeTrueCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            MinValueTextBox.IsEnabled = true;
            MaxValueTextBox.IsEnabled = true;
            UnitsTextBox.IsEnabled = true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ModernMessageDialog.Show("Введите название процедуры", "Предупреждение", MessageType.Warning);
                return;
            }

            if (!double.TryParse(MinValueTextBox.Text, out double minValue))
            {
                ModernMessageDialog.Show("Введите корректное минимальное значение", "Ошибка", MessageType.Error);
                return;
            }

            if (!double.TryParse(MaxValueTextBox.Text, out double maxValue))
            {
                ModernMessageDialog.Show("Введите корректное максимальное значение", "Ошибка", MessageType.Error);
                return;
            }

            bool isTest = MustBeTrueCheckBox.IsChecked == true;
            if (!isTest && minValue >= maxValue)
            {
                ModernMessageDialog.Show("Минимальное значение должно быть меньше максимального", "Ошибка", MessageType.Error);
                return;
            }

            ProcedureName = NameTextBox.Text;
            ProcedureDescription = DescriptionTextBox.Text ?? "";
            MinValue = minValue;
            MaxValue = maxValue;
            Units = UnitsTextBox.Text ?? "";
            MustBeTrue = isTest;

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
