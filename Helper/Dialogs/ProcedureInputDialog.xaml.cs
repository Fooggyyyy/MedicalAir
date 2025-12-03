using System.Windows;
using System.Windows.Input;

namespace MedicalAir.Helper.Dialogs
{
    public partial class ProcedureInputDialog : Window
    {
        public double? Result { get; private set; }

        public ProcedureInputDialog(string procedureName, string description, int minValue, int maxValue, string units)
        {
            InitializeComponent();
            
            DataContext = new ProcedureInputViewModel
            {
                ProcedureName = procedureName,
                Description = description ?? "",
                MinValue = minValue,
                MaxValue = maxValue,
                Units = units ?? "",
                RangeText = $"Допустимый диапазон: {minValue} - {maxValue} {(string.IsNullOrEmpty(units) ? "" : units)}"
            };
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProcedureInputViewModel vm)
            {
                
                if (double.TryParse(vm.Value, out double value))
                {
                    Result = value;
                    DialogResult = true;
                    Close();
                }
                else
                {
                    
                    Result = null;
                    DialogResult = true;
                    Close();
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ValueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OkButton_Click(sender, e);
            }
        }
    }

    public class ProcedureInputViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private string procedureName;
        public string ProcedureName
        {
            get => procedureName;
            set { procedureName = value; OnPropertyChanged(nameof(ProcedureName)); }
        }

        private string description;
        public string Description
        {
            get => description;
            set { description = value; OnPropertyChanged(nameof(Description)); }
        }

        private string value;
        public string Value
        {
            get => value;
            set { this.value = value; OnPropertyChanged(nameof(Value)); }
        }

        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string Units { get; set; }

        private string rangeText;
        public string RangeText
        {
            get => rangeText;
            set { rangeText = value; OnPropertyChanged(nameof(RangeText)); }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
