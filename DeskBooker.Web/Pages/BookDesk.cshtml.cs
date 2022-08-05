using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using DeskBooker.Core.Processor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DeskBooker.Web.Pages;

public class BookDeskModel : PageModel
{
    private readonly IDeskBookingRequestProcessor _deskBookingRequestProcessor;
    private readonly IMeetingRoomRepository _meetingRoomRepository;

    public BookDeskModel(IDeskBookingRequestProcessor deskBookingRequestProcessor, IMeetingRoomRepository meetingRoomRepository)
    {
        _deskBookingRequestProcessor = deskBookingRequestProcessor;
        _meetingRoomRepository = meetingRoomRepository;
    }

    [BindProperty]
    public DeskBookingRequest DeskBookingRequest { get; set; }
    public SelectList MeetingRooms { get; set; }

    public void OnGet()
    {
        DeskBookingRequest = new DeskBookingRequest
        {
            BookingTypeId = (int)BookingTypes.Desk,
            Date = System.DateTime.Now
        };
        var meetingRooms = _meetingRoomRepository.GetAll();
        MeetingRooms = new SelectList(meetingRooms, nameof(MeetingRoom.Id), nameof(MeetingRoom.RoomName));
    }

    public IActionResult OnPost()
    {
        IActionResult actionResult = Page();

        if (ModelState.IsValid)
        {
            var result = _deskBookingRequestProcessor.BookDesk(DeskBookingRequest);

            if (result.Code == DeskBookingResultCode.Success)
            {
                actionResult = RedirectToPage("BookDeskConfirmation", result);
            }
            else if (result.Code == DeskBookingResultCode.RepeatedDeskBooking)
            {
                ModelState.AddModelError("DeskBookingRequest.Email",
                 "There is already a booked desk with the current email in the selected date.");
            }
            else if (result.Code == DeskBookingResultCode.NoDeskAvailable)
            {
                ModelState.AddModelError("DeskBookingRequest.Date",
                  "No desk available for selected date.");
            }
            else if (result.Code == DeskBookingResultCode.MeetingRoomNotAvailable)
            {
                ModelState.AddModelError("DeskBookingRequest.MeetingRoomId",
                 "Meeting Room not available in the selected time frame.");
            }
        }

        var meetingRooms = _meetingRoomRepository.GetAll();
        MeetingRooms = new SelectList(meetingRooms, nameof(MeetingRoom.Id), nameof(MeetingRoom.RoomName));
        return actionResult;
    }
}
