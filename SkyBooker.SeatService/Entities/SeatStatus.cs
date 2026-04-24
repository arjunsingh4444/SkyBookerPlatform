namespace SkyBooker.SeatService.Entities;

// Status of a seat
public enum SeatStatus
{
    Available = 0,     // Seat is free to book
    Locked = 1,        // Temporarily held (user is in booking process)
    Booked = 2         // Seat is confirmed/booked
}
