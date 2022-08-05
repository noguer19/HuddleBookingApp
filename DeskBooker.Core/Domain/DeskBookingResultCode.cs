namespace DeskBooker.Core.Domain;

public enum DeskBookingResultCode
{
    Success,
    NoDeskAvailable,
    MeetingRoomNotAvailable,
    RepeatedDeskBooking
}
