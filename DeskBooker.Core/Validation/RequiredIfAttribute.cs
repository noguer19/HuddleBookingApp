using DeskBooker.Core.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace DeskBooker.Core.Validation;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class RequiredIfAttribute : RequiredAttribute
{
    private string PropertyName { get; }

    public RequiredIfAttribute(string propertyName)
    {
        PropertyName = propertyName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        object instance = context.ObjectInstance;
        Type type = instance.GetType();
        bool parsed = int.TryParse(type.GetProperty(PropertyName).GetValue(instance)?.ToString(), out int bookingTypeId);
        if (parsed && bookingTypeId == (int)BookingTypes.MeetingRoom && string.IsNullOrWhiteSpace(value?.ToString()))
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
