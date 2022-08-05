using DeskBooker.Core.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace DeskBooker.Core.Domain;

public class DeskBookingBase
{
    [Required]
    [StringLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    [DataType(DataType.Date)]
    [DateInFuture]
    [DateWithoutTime]
    public DateTime Date { get; set; }

    [Required]
    [Display(Name = "Booking Type")]
    public int BookingTypeId
    {
        get
        {
            return (int)BookingType;
        }
        set
        {
            BookingType = (BookingTypes)value;
        }
    }

    public string Notes { get; set; }

    [Required]
    public bool Active { get; set; } = true;

    [DataType(DataType.Time)]
    [RequiredIf(nameof(BookingTypeId))]
    [Display(Name = "Booking Start Time")]
    [DateGreaterThan(nameof(BookingEndTime))]
    public DateTime? BookingStartTime { get; set; }

    [DataType(DataType.Time)]
    [RequiredIf(nameof(BookingTypeId))]
    [Display(Name = "Booking End Time")]
    [DateLowerThan(nameof(BookingStartTime))]
    public DateTime? BookingEndTime { get; set; }

    [Display(Name = "Meeting Room")]
    public int? MeetingRoomId { get; set; }

    public BookingTypes BookingType { get; set; }
    public virtual MeetingRoom MeetingRoom { get; set; }
}

public enum BookingTypes
{
    Desk = 1,
    MeetingRoom = 2
}
