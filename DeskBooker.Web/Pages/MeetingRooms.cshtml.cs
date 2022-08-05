using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace DeskBooker.Web.Pages;

public class MeetingRoomsModel : PageModel
{
    private readonly IMeetingRoomRepository _meetingRoomRepository;

    public MeetingRoomsModel(IMeetingRoomRepository meetingRoomRepository)
    {
        _meetingRoomRepository = meetingRoomRepository;
    }

    public IEnumerable<MeetingRoom> MeetingRooms { get; set; }

    public void OnGet()
    {
        MeetingRooms = _meetingRoomRepository.GetAll();
    }
}
