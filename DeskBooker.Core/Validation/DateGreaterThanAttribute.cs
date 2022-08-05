using System;
using System.ComponentModel.DataAnnotations;

namespace DeskBooker.Core.Validation;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class DateGreaterThanAttribute : ValidationAttribute
{
    private string PropertyName { get; }

    public DateGreaterThanAttribute(string propertyName)
    {
        PropertyName = propertyName;
        ErrorMessage = $"Value cannot be greater than {propertyName}.";
    }

    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        object instance = context.ObjectInstance;
        Type type = instance.GetType();
        bool parsed = DateTime.TryParse(type.GetProperty(PropertyName).GetValue(instance)?.ToString(), out DateTime propertyValue);
        if (parsed && propertyValue < (DateTime)value)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}


