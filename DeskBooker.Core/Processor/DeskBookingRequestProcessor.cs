using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using System;
using System.Linq;

namespace DeskBooker.Core.Processor;

public class DeskBookingRequestProcessor : IDeskBookingRequestProcessor
{
    private readonly IDeskBookingRepository _deskBookingRepository;
    private readonly IDeskRepository _deskRepository;

    public DeskBookingRequestProcessor(IDeskBookingRepository deskBookingRepository,
      IDeskRepository deskRepository)
    {
        _deskBookingRepository = deskBookingRepository;
        _deskRepository = deskRepository;
    }

    public DeskBookingResult BookDesk(DeskBookingRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var deskBookingResult = Create<DeskBookingResult>(request);

        if (request.BookingTypeId == (int)BookingTypes.Desk)
        {
            request.MeetingRoomId = null;
            var allBookings = _deskBookingRepository.GetAll();
            var alreadyBooked = allBookings.Any(b => b.Email == request.Email && b.BookingTypeId == (int)BookingTypes.Desk && b.Date == request.Date);
            if (alreadyBooked)
            {
                deskBookingResult.Code = DeskBookingResultCode.RepeatedDeskBooking;
                return deskBookingResult;
            }

            var availableDesks = _deskRepository.GetAvailableDesks(request.Date);
            if (!availableDesks.Any())
            {
                deskBookingResult.Code = DeskBookingResultCode.NoDeskAvailable;
                return deskBookingResult;
            }

            var firstAvailableDesk = availableDesks.FirstOrDefault();
            var deskBooking = Create<DeskBooking>(request);
            deskBooking.DeskId = firstAvailableDesk.Id;
            SaveBooking(deskBookingResult, deskBooking);
        }

        if (request.BookingTypeId == (int)BookingTypes.MeetingRoom)
        {
            var isMeetingRoomAvailable = _deskBookingRepository.IsMeetingRoomAvailable(request.Date, request.BookingStartTime, request.BookingEndTime, request.MeetingRoomId);
            if (!isMeetingRoomAvailable)
            {
                deskBookingResult.Code = DeskBookingResultCode.MeetingRoomNotAvailable;
                return deskBookingResult;
            }

            var deskBooking = Create<DeskBooking>(request);
            SaveBooking(deskBookingResult, deskBooking);
        }

        return deskBookingResult;
    }

    private void SaveBooking(DeskBookingResult deskBookingResult, DeskBooking deskBooking)
    {
        _deskBookingRepository.Save(deskBooking);
        deskBookingResult.DeskBookingId = deskBooking.Id;
        deskBookingResult.Code = DeskBookingResultCode.Success;
    }

    private static T Create<T>(DeskBookingRequest request) where T : DeskBookingBase, new()
    {
        return new T
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Date = request.Date,
            BookingTypeId = request.BookingTypeId,
            Notes = request.Notes,
            BookingStartTime = request.BookingStartTime,
            BookingEndTime = request.BookingEndTime,
            MeetingRoomId = request.MeetingRoomId
        };
    }
}
