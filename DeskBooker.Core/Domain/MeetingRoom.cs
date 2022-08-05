namespace DeskBooker.Core.Domain;

public class MeetingRoom
{
    public int Id { get; set; }
    public string RoomName { get; set; }
    public string RoomDescription { get; set; }
    public int MaxPeopleAllowed { get; set; }
    public bool Active { get; set; }
    public string ImageUrl { get; set; }
}
