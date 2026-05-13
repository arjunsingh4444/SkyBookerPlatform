using System.ComponentModel.DataAnnotations;

namespace SkyBooker.SeatService.Dtos;

// Used when locking or booking a seat
public class SeatActionDto
{
    [Required]
    public int FlightId { get; set; }

    [Required]
    public string SeatNumber { get; set; } = string.Empty;

    [Required]
    public int UserId { get; set; }
}
