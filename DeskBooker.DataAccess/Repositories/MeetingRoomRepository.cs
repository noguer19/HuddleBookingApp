using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using System.Collections.Generic;
using System.Linq;

namespace DeskBooker.DataAccess.Repositories;

public class MeetingRoomRepository : IMeetingRoomRepository
{
    private readonly DeskBookerContext _context;

    public MeetingRoomRepository(DeskBookerContext context)
    {
        _context = context;
    }

    public IEnumerable<MeetingRoom> GetAll()
    {
        return _context.MeetingRoom.ToList();
    }
}
