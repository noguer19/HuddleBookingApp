using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using DeskBooker.Core.Processor;
using Moq;
using Xunit;

namespace DeskBooker.Core.Tests.Processor;

public class DeskBookingRequestProcessorTests
{
    private readonly DeskBookingRequestProcessor _processor;
    private readonly Mock<IDeskBookingRepository> _deskBookingRepositoryMock;
    private readonly Mock<IDeskRepository> _deskRepositoryMock;

    public DeskBookingRequestProcessorTests()
    {
        _deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();
        _deskRepositoryMock = new Mock<IDeskRepository>();
        _processor = new DeskBookingRequestProcessor(_deskBookingRepositoryMock.Object, _deskRepositoryMock.Object);
    }

    [Fact]
    public void ShouldThrowExceptionIfRequestIsNull()
    {
        //Arrange
        DeskBookingRequest? bookingRequest = null;

        //Act
        var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookDesk(bookingRequest));

        //Assert
        Assert.Equal("request", exception.ParamName);
        Assert.Contains("Value cannot be null.", exception.Message);
    }

    [Fact]
    public void ShouldReturnDeskBookingResultWithRequestValues()
    {
        //Arrange
        var request = new DeskBookingRequest
        {
            FirstName = "Thomas",
            LastName = "Huber",
            Email = "thomas@thomasclaudiushuber.com",
            Date = new DateTime(2020, 1, 28),
            BookingTypeId = (int)BookingTypes.Desk,
            Active = true,
        };

        // Act
        DeskBookingResult result = _processor.BookDesk(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.FirstName, result.FirstName);
        Assert.Equal(request.LastName, result.LastName);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Date, result.Date);
        Assert.Equal(request.BookingTypeId, result.BookingTypeId);
        Assert.True(result.Active);
    }

    [Fact]
    public void ShouldNotSaveDeskBookingIfNoDeskIsAvailable()
    {
        //Arrange
        var request = new DeskBookingRequest
        {
            FirstName = "Thomas",
            LastName = "Huber",
            Email = "thomas@thomasclaudiushuber.com",
            Date = new DateTime(2020, 1, 28),
            BookingTypeId = (int)BookingTypes.Desk,
            Active = true,
        };

        var availableDesks = new List<Desk>();
        _deskRepositoryMock.Setup(x => x.GetAvailableDesks(request.Date)).Returns(availableDesks);

        //Act
        _processor.BookDesk(request);

        //Assert
        _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
    }

    [Fact]
    public void ShouldNotSaveDeskBookingIfUserAlreadyBookADesk()
    {
        //Arrange
        var request = new DeskBookingRequest
        {
            FirstName = "Thomas",
            LastName = "Huber",
            Email = "thomas@thomasclaudiushuber.com",
            Date = new DateTime(2020, 1, 28),
            BookingTypeId = (int)BookingTypes.Desk,
            Active = true,
        };

        var listOfBookings = new List<DeskBooking>
        {
            new DeskBooking { FirstName = request.FirstName, LastName = request.LastName, Email = request.Email, Date = request.Date, BookingTypeId = request.BookingTypeId },
            new DeskBooking { FirstName = "Luis", LastName = "Noguera", Email = "lnoguera@cmnhospitals.org", Date = new DateTime(2020, 1, 30), BookingTypeId = request.BookingTypeId },
            new DeskBooking { FirstName = "Carlos", LastName = "Martinez", Email = "cmartinez@gmail.com", Date = new DateTime(2020, 1, 31), BookingTypeId = request.BookingTypeId },
        };

        _deskBookingRepositoryMock.Setup(x => x.GetAll()).Returns(listOfBookings);

        //Act
        _processor.BookDesk(request);

        //Assert
        _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
    }

    [Fact]
    public void ShouldSaveDeskBooking()
    {
        //Arrange
        var request = new DeskBookingRequest
        {
            FirstName = "Thomas",
            LastName = "Huber",
            Email = "thomas@thomasclaudiushuber.com",
            Date = new DateTime(2020, 1, 28),
            BookingTypeId = (int)BookingTypes.Desk,
            Active = true,
        };

        var availableDesks = new List<Desk>
        {
            new Desk { Id = 1},
            new Desk { Id = 2},
            new Desk { Id = 3}
        };

        _deskRepositoryMock.Setup(x => x.GetAvailableDesks(request.Date)).Returns(availableDesks);

        DeskBooking savedDeskBooking = null;
        _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
          .Callback<DeskBooking>(deskBooking =>
          {
              savedDeskBooking = deskBooking;
          });

        //Act
        _processor.BookDesk(request);

        //Assert
        _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);

        Assert.NotNull(savedDeskBooking);
        Assert.Equal(request.FirstName, savedDeskBooking.FirstName);
        Assert.Equal(request.LastName, savedDeskBooking.LastName);
        Assert.Equal(request.Email, savedDeskBooking.Email);
        Assert.Equal(request.Date, savedDeskBooking.Date);
        Assert.Equal(availableDesks.First().Id, savedDeskBooking.DeskId);
    }


    [Fact]
    public void ShouldNotSaveMeetingRoomBookingIfMeetingRoomNotAvailable()
    {
        //Arrange
        var request = new DeskBookingRequest
        {
            FirstName = "Thomas",
            LastName = "Huber",
            Email = "thomas@thomasclaudiushuber.com",
            Date = new DateTime(2020, 1, 28),
            BookingTypeId = (int)BookingTypes.MeetingRoom,
            Active = true,
            BookingStartTime = new DateTime(2020, 1, 28, 10, 0, 0),
            BookingEndTime = new DateTime(2020, 1, 28, 11, 0, 0),
            MeetingRoomId = 1
        };

        _deskBookingRepositoryMock.Setup(x => x.IsMeetingRoomAvailable(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns(false);

        //Act
        var result = _processor.BookDesk(request);

        //Assert
        _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
        Assert.Equal(DeskBookingResultCode.MeetingRoomNotAvailable, result.Code);
    }

    [Fact]
    public void ShouldSaveMeetingRoomBookingIfMeetingRoomAvailable()
    {
        //Arrange
        var request = new DeskBookingRequest
        {
            FirstName = "Thomas",
            LastName = "Huber",
            Email = "thomas@thomasclaudiushuber.com",
            Date = new DateTime(2020, 1, 28),
            BookingTypeId = (int)BookingTypes.MeetingRoom,
            Active = true,
            BookingStartTime = new DateTime(2020, 1, 28, 10, 0, 0),
            BookingEndTime = new DateTime(2020, 1, 28, 11, 0, 0),
            MeetingRoomId = 1
        };

        _deskBookingRepositoryMock.Setup(x => x.IsMeetingRoomAvailable(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
            .Returns(true);

        //Act
        var result = _processor.BookDesk(request);

        //Assert
        _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);
        Assert.Equal(DeskBookingResultCode.Success, result.Code);
    }

    [Theory]
    [InlineData(BookingTypes.Desk, true, null, DeskBookingResultCode.Success)]
    [InlineData(BookingTypes.Desk, false, null, DeskBookingResultCode.NoDeskAvailable)]
    [InlineData(BookingTypes.MeetingRoom, null, true, DeskBookingResultCode.Success)]
    [InlineData(BookingTypes.MeetingRoom, null, false, DeskBookingResultCode.MeetingRoomNotAvailable)]
    public void ShouldReturnExpectedResultCode(BookingTypes bookingType, bool? isDeskAvailable, bool? isMeetingRoomAvailable, DeskBookingResultCode expectedResultCode)
    {
        //Arrange
        var request = new DeskBookingRequest
        {
            FirstName = "Thomas",
            LastName = "Huber",
            Email = "thomas@thomasclaudiushuber.com",
            Date = new DateTime(2020, 1, 28),
            Active = true,
        };

        var availableDesks = new List<Desk>
        {
            new Desk { Id = 1},
            new Desk { Id = 2},
            new Desk { Id = 3}
        };

        if (isDeskAvailable.HasValue && !isDeskAvailable.Value)
        {
            availableDesks.Clear();
        }

        if (bookingType == BookingTypes.Desk)
        {
            request.BookingTypeId = (int)BookingTypes.Desk;
            _deskRepositoryMock.Setup(x => x.GetAvailableDesks(It.IsAny<DateTime>())).Returns(availableDesks);
        }

        bool _isMeetingRoomAvailable = true;
        if (isMeetingRoomAvailable.HasValue && !isMeetingRoomAvailable.Value)
        {
            _isMeetingRoomAvailable = false;
        }

        if (bookingType == BookingTypes.MeetingRoom)
        {
            request.BookingTypeId = (int)BookingTypes.MeetingRoom;
            request.MeetingRoomId = 1;
            request.BookingStartTime = new DateTime(2020, 1, 28, 10, 0, 0);
            request.BookingEndTime = new DateTime(2020, 1, 28, 11, 0, 0);
            _deskBookingRepositoryMock.Setup(x => x.IsMeetingRoomAvailable(request.Date, request.BookingStartTime, request.BookingEndTime, request.MeetingRoomId))
                .Returns(_isMeetingRoomAvailable);
        }

        var result = _processor.BookDesk(request);

        Assert.Equal(expectedResultCode, result.Code);
    }



}
