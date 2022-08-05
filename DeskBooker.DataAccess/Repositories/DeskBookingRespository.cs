using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeskBooker.DataAccess.Repositories;

public class DeskBookingRepository : IDeskBookingRepository
{
    private readonly DeskBookerContext _context;

    public DeskBookingRepository(DeskBookerContext context)
    {
        _context = context;
    }

    public IEnumerable<DeskBooking> GetAll()
    {
        return _context.DeskBooking.Include(d => d.MeetingRoom).OrderBy(x => x.Date).ToList();
    }

    public void Save(DeskBooking deskBooking)
    {
        _context.DeskBooking.Add(deskBooking);
        _context.SaveChanges();
    }

    public bool IsMeetingRoomAvailable(DateTime date, DateTime startTime, DateTime endTime, int meetingRoomId)
    {
        var meetingRoomBookings = _context.DeskBooking.Where(d => d.BookingTypeId == (int)BookingTypes.MeetingRoom && d.Date == date && d.MeetingRoomId == meetingRoomId).ToList();
        bool result = true;
        foreach (var booking in meetingRoomBookings)
        {
            var newBookingStartTimeOnly = startTime.TimeOfDay;
            var newBookingEndTimeOnly = endTime.TimeOfDay;
            var existingBookingStartTimeOnly = booking.BookingStartTime.Value.TimeOfDay;
            var existingBookingEndTimeOnly = booking.BookingEndTime.Value.TimeOfDay;
            if ((newBookingStartTimeOnly >= existingBookingStartTimeOnly && newBookingStartTimeOnly < existingBookingEndTimeOnly) ||
                (newBookingEndTimeOnly >= existingBookingStartTimeOnly && newBookingStartTimeOnly < existingBookingEndTimeOnly))
            {
                result = false;
                break;
            }
        }

        return result;
    }
}
