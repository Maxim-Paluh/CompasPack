using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CompasPack.Wrapper
{
    public class ModelWrapper<T> : NotifyDataErrorInfoBase where T : class
    {
        public T Model { get; set; }
        public ModelWrapper(T model)
        {
            Model = model;
        }
        public void ValidateAllCustomErrors()
        {
            foreach (var property in this.GetType().GetProperties())
            {
                ValidateCustomErrors(property.Name);
            }

        }
        protected virtual void SetValue<TValue>(TValue value, [CallerMemberName]string? propertyName = null)
        {
            if (propertyName != null)
            {
                typeof(T).GetProperty(propertyName)?.SetValue(Model, value);
                OnPropertyChanged(propertyName);
                ValidatePropertyInternal(propertyName, value);
            }
        }
        protected virtual TValue? GetValue<TValue>([CallerMemberName]string? propertyName = null)
        {
            if (propertyName != null)
                return (TValue?)typeof(T).GetProperty(propertyName)?.GetValue(Model);
            else
                return default;
        }
        private void ValidatePropertyInternal(string propertyName, object? currentValue)
        {
            ClearError(propertyName);

            ValidateDataAnnotations(propertyName, currentValue);

            ValidateCustomErrors(propertyName);
        }
        private void ValidateDataAnnotations(string propertyName, object? currentValue)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(Model) { MemberName = propertyName };

            Validator.TryValidateProperty(currentValue, context, results);

            foreach (var validationResult in results)
            {
                AddError(propertyName, validationResult?.ErrorMessage!=null? validationResult.ErrorMessage: string.Empty);
            }
        }
        private void ValidateCustomErrors(string propertyName)
        {
            var errors = ValidateProperty(propertyName);
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    AddError(propertyName, error);
                }
            }
        }
        protected virtual IEnumerable<string>? ValidateProperty(string propertyName)
        {
            return null;
        }
    }
}
