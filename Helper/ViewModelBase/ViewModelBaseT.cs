using FluentValidation;
using FluentValidation.Results;
using MedicalAir.Helper.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicalAir.Helper.ViewModelBase
{
    public abstract class ViewModelBase<T> : INotifyPropertyChanged, INotifyDataErrorInfo
     where T : class
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool Set<TField>(ref TField field, TField value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TField>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            ValidateProperty(propertyName);
            return true;
        }

        protected ICommand CreateAsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
           => new RelayCommand(execute, canExecute);

        protected ICommand CreateAsyncCommand(Func<object, Task> execute, Func<object, bool>? canExecute = null)
            => new RelayCommand(execute, canExecute);

        protected ICommand CreateCommand(Action<object> execute, Func<object, bool>? canExecute = null)
            => new RelayCommand(execute, canExecute);

        protected IValidator<T>? Validator { get; set; }
        private readonly Dictionary<string, List<string>> _errors = new();

        public bool HasErrors => _errors.Any();
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
            => propertyName != null && _errors.ContainsKey(propertyName)
                ? _errors[propertyName]
                : Enumerable.Empty<string>();

        protected void ValidateProperty(string propertyName)
        {
            if (Validator == null) return;

            ValidationResult result = Validator.Validate((T)(object)this); 

            _errors.Remove(propertyName);

            var propErrors = result.Errors
                .Where(e => e.PropertyName == propertyName)
                .Select(e => e.ErrorMessage)
                .ToList();

            if (propErrors.Any())
                _errors[propertyName] = propErrors;

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }


}
