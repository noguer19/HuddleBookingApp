using DeskBooker.Core.Domain;
using System.Collections.Generic;

namespace DeskBooker.Core.DataInterface;

public interface IMeetingRoomRepository
{
    IEnumerable<MeetingRoom> GetAll();
}
