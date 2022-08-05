using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using DeskBooker.Core.Processor;
using DeskBooker.Web.Pages;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeskBooker.Web.Tests.Pages;

public class BookDeskModelTests
{
    private readonly Mock<IDeskBookingRequestProcessor> _deskBookingRequestProcessorMock;
    private readonly Mock<IMeetingRoomRepository> _meetingRoomRepositoryMock;
    private readonly BookDeskModel bookDeskModel;

    public BookDeskModelTests()
    {
        _deskBookingRequestProcessorMock = new Mock<IDeskBookingRequestProcessor>();
        _meetingRoomRepositoryMock = new Mock<IMeetingRoomRepository>();
        bookDeskModel = new BookDeskModel(_deskBookingRequestProcessorMock.Object, _meetingRoomRepositoryMock.Object);
    }

    [Fact]
    public void OnGetShouldGetAllMeetingRooms()
    {
        // Arrange
        var meetingRooms = new List<MeetingRoom>
        {
            new MeetingRoom { Id = 1, RoomName = "Momotombo", Active = true, MaxPeopleAllowed = 10 },
            new MeetingRoom { Id = 2, RoomName = "Sacuanjoche", Active = true, MaxPeopleAllowed = 4 },
            new MeetingRoom { Id = 1, RoomName = "Guardabarranco", Active = true, MaxPeopleAllowed = 6 },
        };

        _meetingRoomRepositoryMock.Setup(r => r.GetAll()).Returns(meetingRooms);

        //Act
        bookDeskModel.OnGet();

        //Assert

        Assert.Contains(bookDeskModel.MeetingRooms, item => item.Text == meetingRooms.FirstOrDefault().RoomName);
        Assert.Contains(bookDeskModel.MeetingRooms, item => item.Value == meetingRooms.FirstOrDefault().Id.ToString());
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    public void OnPostShouldCallBookDeskMethodOfProcessorIfModelIsValid(int expectedBookDeskCalls, bool isModelValid)
    {
        // Arrange
        if (!isModelValid)
        {
            bookDeskModel.ModelState.AddModelError("JustAKey", "AnErrorMessage");
        }

        _deskBookingRequestProcessorMock.Setup(m => m.BookDesk(It.IsAny<DeskBookingRequest>()))
            .Returns(new DeskBookingResult { Code = DeskBookingResultCode.Success });

        // Act
        bookDeskModel.OnPost();

        // Assert
        _deskBookingRequestProcessorMock.Verify(x => x.BookDesk(bookDeskModel.DeskBookingRequest),
          Times.Exactly(expectedBookDeskCalls));
    }

    [Fact]
    public void OnPostShouldAddModelErrorIfNoDeskIsAvailable()
    {
        // Arrange
        _deskBookingRequestProcessorMock.Setup(m => m.BookDesk(It.IsAny<DeskBookingRequest>()))
            .Returns(new DeskBookingResult { Code = DeskBookingResultCode.NoDeskAvailable });

        // Act
        bookDeskModel.OnPost();

        // Assert
        var modelStateEntry = Assert.Contains("DeskBookingRequest.Date", bookDeskModel.ModelState);
        var modelError = Assert.Single(modelStateEntry.Errors);
        Assert.Equal("No desk available for selected date.", modelError.ErrorMessage);
    }

}
