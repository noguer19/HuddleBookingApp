using DeskBooker.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeskBooker.Core.DataInterface;

public interface IDeskBookingRepository
{
    void Save(DeskBooking deskBooking);
    IEnumerable<DeskBooking> GetAll();
    bool IsMeetingRoomAvailable(DateTime date, DateTime startTime, DateTime endTime, int meetingRoomId);
}
